using Application.Services.Interfaces;
using Application.Client.Marzban;
using AutoMapper;
using Domain.Contract;
using Domain.DTOs.Node;
using Domain.Entities;
using Domain.Enumes.Node; // needed for provisioning enums
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations;

public class NodeService : INodeService
{
    private readonly IAsyncRepository<Node, long> repository;
    private readonly ILogger<INodeService> logger;
    private readonly IAsyncRepository<Instance, long> instanceRepository;
    private readonly IUserContextService userContextService;
    private readonly IUserService userService;
    private readonly IMapper mapper;
    private readonly IMarzbanApiClient marzbanApiClient;

    public NodeService(
        IAsyncRepository<Node, long> repository,
        ILogger<INodeService> logger,
        IAsyncRepository<Instance, long> instanceRepository,
        IUserContextService userContextService,
        IUserService userService,
        IMapper mapper,
        IMarzbanApiClient marzbanApiClient)
    {
        this.repository = repository;
        this.logger = logger;
        this.instanceRepository = instanceRepository;
        this.userContextService = userContextService;
        this.userService = userService;
        this.mapper = mapper;
        this.marzbanApiClient = marzbanApiClient;
    }

    public async Task<NodeDto> GetNodeByIdAsync(long id)
    {
        logger.LogInformation("get node by id: {id}", id);
        var node = await repository.GetByIdAsync(id);
        return mapper.Map<NodeDto>(node);
    }

    public async Task<List<NodeDto>> GetNodesAsync()
    {
        logger.LogCritical("get all nodes by rules to show for users");

        var user = await userService.GetUserByIdAsync(userContextService.UserId);
        var price = user.PriceMultiplier;

        var nodes = await repository.GetQuery()
            .Include(n => n.Instances)
            .Where(n =>
                n.IsAvailableForShow &&
                n.Instances.Count < 10)
            .ToListAsync();

        var dto = mapper.Map<List<NodeDto>>(nodes);

        foreach (var n in dto) n.Price *= price;

        return dto;
    }

    public async Task<NodeDto> CreateNodeAsync(NodeCreateDto create)
    {
        logger.LogInformation("create a node");

        var exist = await repository.GetSingleAsync(n => n.NodeName == create.NodeName);

        if (exist != null) throw new ExistsException("node already exists");

        var node = mapper.Map<Node>(create);

        await repository.AddEntity(node);
        await repository.SaveChangesAsync();

        logger.LogInformation("create a node with id: {id}", node.Id);

        return mapper.Map<NodeDto>(node);
    }

    public async Task<List<NodeDto>> GetNodesForPanelAsync(long panelId)
    {
        var panels = await instanceRepository.GetQuery()
            .Where(i => i.PanelId == panelId && i.UserId == userContextService.UserId)
            .Include(i => i.Node)
            .Select(i => i.Node.NodeName)
            .ToListAsync();

        return mapper.Map<List<NodeDto>>(panels);
    }

    public async Task DeleteNodeAsync(long nodeId)
    {
        logger.LogInformation($"Deleting node and all related instances: {nodeId}");
        var node = await repository.GetQuery()
            .Include(n => n.Instances).ThenInclude(i => i.Panel)
            .FirstOrDefaultAsync(n => n.Id == nodeId) ?? throw new NotFoundException("Node not found");
        foreach (var instance in node.Instances.ToList())
        {
            try
            {
                var panel = instance.Panel;
                if (instance.MarzbanNodeId != 0)
                    await marzbanApiClient.DeleteNodeAsync(panel.Url, panel.Token, instance.MarzbanNodeId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete Marzban node for instance {InstanceId}", instance.Id);
            }
            instanceRepository.DeleteEntity(instance);
        }
        await instanceRepository.SaveChangesAsync();
        repository.DeleteEntity(node);
        await repository.SaveChangesAsync();
        logger.LogInformation("Node {NodeId} and related instances deleted", nodeId);
    }

    public async Task<List<Node>> GetProvisioningCandidatesAsync()
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-10);
        return await repository.GetQuery()
            .Where(n => n.IsEnabled && (
                n.ProvisioningStatus == NodeProvisioningStatus.Pending ||
                (n.ProvisioningStatus == NodeProvisioningStatus.Installing && (n.LastSeenUtc == null || n.LastSeenUtc < cutoff))
            ))
            .ToListAsync();
    }

    public async Task UpdateNodeAsync(Node node)
    {
        await repository.UpdateEntity(node);
        await repository.SaveChangesAsync();
    }

    public async Task<List<NodeDto>> GetAllAdminNodesAsync()
    {
        var nodes = await repository.GetQuery().Include(n => n.Instances).ToListAsync();
        return mapper.Map<List<NodeDto>>(nodes);
    }
}
