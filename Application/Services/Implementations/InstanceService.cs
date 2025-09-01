using System.Linq.Expressions;
using Application.Client.Marzban;
using Application.Client.NodeManager;
using Application.Models.Marzban;
using Application.Models.NodeManager;
using Application.Services.Interfaces;
using Application.Statics; // <-- for structured business logs
using AutoMapper;
using Domain.Contract;
using Domain.DTOs.Instance;
using Domain.DTOs.Panel;
using Domain.DTOs.Transaction;
using Domain.Entities;
using Domain.Enumes.Container;
using Domain.Enumes.Transaction;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations;

public class InstanceService(
    IAsyncRepository<Instance, long> repository,
    IMapper mapper,
    ITransactionService transactionService,
    ILogger<IInstanceService> logger,
    IUserService userService,
    IServiceProvider serviceProvider,
    IUserContextService userContextService) : IInstanceService
{
    public async Task<InstanceDto> CreateInstanceAsync(InstanceCreateDto create)
    {
        var instance = mapper.Map<Instance>(create);

        await repository.AddEntity(instance);
        await repository.SaveChangesAsync();

        // business log
        logger.InstanceCreated(
            instanceId: instance.Id,
            userId: instance.UserId,
            nodeId: instance.NodeId,
            panelId: instance.PanelId
        );

        return mapper.Map<InstanceDto>(instance);
    }

    public async Task<InstanceDto?> GetInstanceByIdAsync(long id)
    {
        var instance = await repository.GetByIdAsync(id);
        return mapper.Map<InstanceDto>(instance);
    }

    public async Task<InstanceDto?> GetInstanceByProvisionedIdAsync(string provisionedId)
    {
        var instance = await repository.GetSingleAsync(i => i.ProvisionedInstanceId == provisionedId);
        return mapper.Map<InstanceDto>(instance);
    }

    public async Task<string> UpdateInstanceStatusAsync(long id, ContainerProvisionStatus newStatus)
    {
        var instance = await repository.GetByIdAsync(id)
                      ?? throw new NotFoundException($"instance with ID {id} not found.");

        var oldStatus = instance.Status.ToString();
        instance.Status = newStatus;

        await repository.UpdateEntity(instance);
        await repository.SaveChangesAsync();

        // business log
        logger.InstanceStatusChanged(
            instanceId: id,
            oldStatus: oldStatus,
            newStatus: newStatus.ToString()
        );

        return $"instance {id} status updated to {newStatus}.";
    }

    public async Task ProcessUsageReportAsync(UsageReportDto report)
    {
        var instanceIdsFromReport = report.Usages.Select(u => u.InstanceId).ToList();
        if (!instanceIdsFromReport.Any()) return;

        var includes = new List<Expression<Func<Instance, object>>>
        {
            i => i.User,
            i => i.Node
        };

        var instances = await repository.GetAsync(
            predicate: i => instanceIdsFromReport.Contains(i.Id),
            includes: includes
        );

        foreach (var instance in instances)
        {
            if (instance.Status != ContainerProvisionStatus.Running)
                continue;

            var usageData = report.Usages.FirstOrDefault(u => u.InstanceId == instance.Id);
            if (usageData == null) continue;

            var usageToBill = usageData.TotalUsageInBytes - instance.LastBilledUsage;
            if (usageToBill <= 0) continue;

            var cost = CalculateCostFromUsage(usageToBill, instance.Node.Price);
            if (cost <= 0) continue;

            if (instance.User.Balance < cost)
                logger.LogWarning("Insufficient balance for user {UserId}.", instance.UserId);

            var usageInGb = usageToBill / (1024m * 1024m * 1024m);
            var description =
                $"Deduction for {usageInGb:F2} GB of usage on service {instance.Node.NodeName} from {instance.LastBillingTimestamp:g} to {DateTime.UtcNow:g}";

            var transactionDto = new TransactionCreateDto
            {
                UserId = instance.UserId,
                Amount = cost,
                Type = TransactionType.Debit,
                Reason = TransactionReason.ServiceUsage,
                Description = description
            };

            try
            {
                await transactionService.CreateTransactionAsync(transactionDto);
                instance.LastBilledUsage = usageData.TotalUsageInBytes;
                instance.LastBillingTimestamp = DateTime.UtcNow;
                await repository.UpdateEntity(instance);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Failed to process financial transaction for instance {InstanceId}. Usage stats will not be updated to allow for retry.",
                    instance.Id);
            }
        }

        await repository.SaveChangesAsync();
    }

    public async Task ResumeStopedServices(long userId)
    {
        using var scope = serviceProvider.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<INodeManagerApiClient>();

        var user = await userService.GetUserByIdAsync(userId);
        if ((user.Balance + user.Credit) <= 0)
            return;

        var suspendedInstances = await repository.GetQuery()
            .Where(i => i.UserId == userId && i.Status == ContainerProvisionStatus.PausedBySystem)
            .Include(i => i.Node)
            .ToListAsync();
        if (suspendedInstances.Count == 0) return;

        foreach (var instance in suspendedInstances)
        {
            try
            {
                var resumeRequest = new ContainerActionRequestDto
                {
                    NodeId = instance.NodeId,
                    ContainerId = instance.ContainerDockerId
                };
                await client.ResumeContainerAsync(resumeRequest);

                instance.Status = ContainerProvisionStatus.Running;
                await repository.UpdateEntity(instance);

                logger.InstanceResumed(
                    instanceId: instance.Id,
                    userId: userId,
                    by: "System"
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to resume instance {InstanceId} for user {UserId}",
                    instance.Id, userId);
            }

            await repository.SaveChangesAsync();
        }
    }

    private long CalculateCostFromUsage(long usedTrafficBytes, decimal pricePerGb)
    {
        if (usedTrafficBytes <= 0 || pricePerGb <= 0) return 0;

        const decimal bytesInGb = 1024m * 1024m * 1024m;
        var usageInGb = usedTrafficBytes / bytesInGb;
        var totalCost = usageInGb * pricePerGb;

        return (long)Math.Ceiling(totalCost);
    }

    public async Task CheckAndSuspendInstancesAsync(long userId)
    {
        using var scope = serviceProvider.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<INodeManagerApiClient>();
        var user = await userService.GetUserByIdAsync(userId);

        if ((user.Balance + user.Credit) > 0) return;

        var activeInstances = await repository.GetAsync(
            i => i.UserId == userId && i.Status == ContainerProvisionStatus.Running,
            includeString: "Node"
        );
        if (!activeInstances.Any()) return;

        foreach (var instance in activeInstances)
        {
            try
            {
                var services = new ContainerActionRequestDto
                {
                    ContainerId = instance.ContainerDockerId,
                    NodeId = instance.Node.Id,
                };

                await client.PauseContainerAsync(services);

                instance.Status = ContainerProvisionStatus.PausedBySystem;
                await repository.UpdateEntity(instance);

                logger.InstanceSuspended(
                    instanceId: instance.Id,
                    userId: userId,
                    reason: "BalanceBelowThreshold"
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to suspend instance {InstanceId} for user {UserId}", instance.Id, userId);
            }
        }

        await repository.SaveChangesAsync();
    }

    public async Task ManuallyPauseInstanceAsync(long instanceId)
    {
        using var scope = serviceProvider.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<INodeManagerApiClient>();

        var instance =
            await repository.GetSingleAsync(i => i.Id == instanceId &&
                                                 i.UserId == userContextService.UserId &&
                                                 i.Status == ContainerProvisionStatus.Running);

        if (instance == null) throw new NotFoundException("Instance not found.");

        var request = new ContainerActionRequestDto
        {
            ContainerId = instance.ContainerDockerId,
            NodeId = instance.NodeId,
        };
        await client.PauseContainerAsync(request);

        var oldStatus = instance.Status.ToString();
        instance.Status = ContainerProvisionStatus.PausedByUser;
        await repository.UpdateEntity(instance);
        await repository.SaveChangesAsync();
        
        logger.InstanceSuspended(
            instanceId: instanceId,
            userId: userContextService.UserId,
            reason: "UserRequest"
        );

        logger.InstanceStatusChanged(instanceId, oldStatus, instance.Status.ToString());
    }

    public async Task ManuallyResumeInstanceAsync(long instanceId)
    {
        using var scope = serviceProvider.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<INodeManagerApiClient>();

        var instance =
            await repository.GetSingleAsync(i => i.Id == instanceId &&
                                                 i.UserId == userContextService.UserId &&
                                                 i.Status == ContainerProvisionStatus.PausedByUser);
        if (instance == null)
            throw new NotFoundException($"Instance with ID {instanceId} not found for this user.");

        if (instance.Status != ContainerProvisionStatus.PausedByUser)
            throw new BadRequestException("This instance is not in a paused state and cannot be resumed.");

        var resumeRequest = new ContainerActionRequestDto
        {
            NodeId = instance.NodeId,
            ContainerId = instance.ContainerDockerId
        };
        await client.ResumeContainerAsync(resumeRequest);

        instance.Status = ContainerProvisionStatus.Running;
        await repository.UpdateEntity(instance);
        await repository.SaveChangesAsync();

        logger.InstanceResumed(
            instanceId: instanceId,
            userId: userContextService.UserId,
            by: "User"
        );
    }

    public async Task<InstanceDto> CreatePreliminaryInstanceAsync(InstanceCreateDto instance)
    {
        var newInstance = mapper.Map<Instance>(instance);

        await repository.AddEntity(newInstance);
        await repository.SaveChangesAsync();

        logger.InstanceCreated(
            instanceId: newInstance.Id,
            userId: newInstance.UserId,
            nodeId: newInstance.NodeId,
            panelId: newInstance.PanelId
        );

        return mapper.Map<InstanceDto>(newInstance);
    }

    public async Task FinalizeInstanceAsync(long instanceId, ProvisionResponseDto response, PanelDto panel)
    {
        var instance = await repository.GetByIdAsync(instanceId);
        if (instance != null)
        {
            var oldStatus = instance.Status.ToString();

            instance.ContainerDockerId = response.ContainerDockerId;
            instance.ProvisionedInstanceId = instanceId.ToString();

            instance.InboundPort = panel.InboundPort;
            instance.XrayPort = panel.XrayPort;
            instance.ApiPort = panel.ApiPort;

            instance.Status = ContainerProvisionStatus.Running;
            instance.LastUpdatedAt = DateTime.UtcNow;

            await repository.UpdateEntity(instance);
            await repository.SaveChangesAsync();

            logger.InstanceStatusChanged(
                instanceId: instanceId,
                oldStatus: oldStatus,
                newStatus: instance.Status.ToString()
            );
        }
    }

    public async Task DeleteInstanceAsync(long instanceId)
    {
        using var scope = serviceProvider.CreateScope();
        var client = scope.ServiceProvider.GetRequiredService<INodeManagerApiClient>();
        var service = scope.ServiceProvider.GetRequiredService<IMarzbanApiClient>();

        var instance = await repository.GetQuery()
            .Include(i => i.Panel)
            .Include(i => i.Node)
            .SingleOrDefaultAsync(i => i.Id == instanceId && i.UserId == userContextService.UserId);

        if (instance == null)
            throw new NotFoundException("Instance not found or you don't have permission.");

        try
        {
            await client.DeprovisionInstanceAsync(instance.Node.SshHost, instanceId);
        }
        catch (Exception e)
        {
            logger.LogInformation(e, "Failed to deprovision instance on NodeManager, but continuing deletion process.");
        }

        try
        {
            var panel = mapper.Map<PanelDto>(instance.Panel);
            var token = instance.Panel.Token;

            var hostsByTag = await service.GetHostsAsync(new MarzbanNodeGetSettingRequest
                { Path = panel.Url, Token = token });
            bool hostsModified = false;

            foreach (var tag in hostsByTag.Keys)
            {
                var hostToRemove = hostsByTag[tag].FirstOrDefault(h => h.Address == instance.Node.SshHost);
                if (hostToRemove != null)
                {
                    hostsByTag[tag].Remove(hostToRemove);
                    hostsModified = true;
                }
            }

            if (hostsModified)
            {
                await service.ModifyHostsAsync(panel.Url, token, hostsByTag);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not remove host from Marzban inbounds. Continuing deletion process.");
        }

        try
        {
            if (instance.MarzbanNodeId != null)
            {
                await service.DeleteNodeAsync(instance.Panel.Url, instance.Panel.Token, instance.MarzbanNodeId);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not remove node definition from Marzban. Continuing deletion process.");
        }

        repository.DeleteEntity(instance);
        await repository.SaveChangesAsync();

        logger.InstanceDeleted(
            instanceId: instanceId,
            userId: userContextService.UserId
        );
    }

    public async Task<List<InstanceDto>> GetAllUserInstancesAsync()
    {
        var instances = await repository.GetQuery()
            .Where(i => i.UserId == userContextService.UserId)
            .ToListAsync();

        return mapper.Map<List<InstanceDto>>(instances);
    }

    public async Task<List<InstanceDto>> GetAllInstancesAsync()
    {
        var instances = await repository.GetAllAsync();
        return mapper.Map<List<InstanceDto>>(instances);
    }

    public async Task<List<InstanceDto>> GetNodesByPanelIdAsync(long panelId)
    {
        var instances = await repository.GetQuery()
            .Where(i =>
                i.PanelId == panelId &&
                i.UserId == userContextService.UserId &&
                i.Node.IsAvailableForShow == true)
            .Include(i => i.Node)
            .ToListAsync();

        if (instances is null)
            throw new NotFoundException("no service found for this panel ");

        return mapper.Map<List<InstanceDto>>(instances);
    }
}
