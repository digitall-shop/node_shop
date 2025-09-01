using Application.Client.NodeManager;
using Application.DomainEvents.Events;
using Application.Models.NodeManager;
using Application.Services.Interfaces;
using Application.Statics; // <<— business logs
using AutoMapper;
using Domain.Contract;
using Domain.DTOs.Instance;
using Domain.Enumes.Container;
using Domain.Events.DomainEvents.Events;
using Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations;

public class NodeProvisioningService(
    IUserContextService userContextService,
    IMapper mapper,
    ILogger<INodeProvisioningService> logger,
    INodeService nodeService,
    IInstanceService instanceService,
    INodeManagerApiClient nodeManagerApiClient,
    IPanelService panelService,
    IMediator mediator) : INodeProvisioningService
{
    public async Task<InstanceDto> ProvisionAsync(long nodeId, long panelId)
    {
        logger.InstanceProvisionRequested(
            userId: userContextService.UserId,
            nodeId: nodeId,
            panelId: panelId
        );

        var panel = await panelService.GetPanelAsync(panelId) ?? throw new NotFoundException("Panel not found");
        var node  = await nodeService.GetNodeByIdAsync(nodeId) ?? throw new NotFoundException("Node not found.");

        var preliminaryInstance = new InstanceCreateDto
        {
            UserId = userContextService.UserId,
            NodeId = nodeId,
            PanelId = panelId,
            Status = ContainerProvisionStatus.Provisioning,
        };

        var newInstanceEntity = await instanceService.CreatePreliminaryInstanceAsync(preliminaryInstance);
        var realInstanceId = newInstanceEntity.Id;

        try
        {
            var provisionRequest = mapper.Map<ProvisionRequestDto>(node, opts =>
            {
                opts.Items["Panel"] = panel;
            });

            provisionRequest.InstanceId = realInstanceId;
            provisionRequest.CustomerId = userContextService.UserId;

            logger.NodeProvisionRequestSent(
                instanceId: realInstanceId,
                nodeId: nodeId,
                panelId: panelId
            );

            var response = await nodeManagerApiClient.ProvisionContainerAsync(provisionRequest);

            logger.NodeProvisionResponse(
                instanceId: realInstanceId,
                isSuccess: response.IsSuccess
            );

            if (!response.IsSuccess)
                throw new BadRequestException("could not provision container");

            
            await instanceService.FinalizeInstanceAsync(realInstanceId, response, panel);

            var provisionedEvent = new InstanceProvisionedEvent(realInstanceId, panelId, nodeId);
            await mediator.Publish(provisionedEvent);

            logger.InstanceProvisionCompleted(realInstanceId);

            return await instanceService.GetInstanceByIdAsync(realInstanceId)
                   ?? throw new NotFoundException("...");
        }
        catch (Exception ex)
        {
            await instanceService.UpdateInstanceStatusAsync(realInstanceId, ContainerProvisionStatus.Failed);
            logger.InstanceProvisionFailed(realInstanceId, ex);
            throw;
        }
    }
}
