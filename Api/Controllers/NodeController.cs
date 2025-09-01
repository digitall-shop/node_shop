using Api.Filters;
using Api.Securities;
using Application.Client.Marzban;
using Application.Models.Marzban;
using Application.Services.Interfaces;
using Domain.DTOs.Instance;
using Domain.DTOs.Node;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Tags("Node Endpoint")]
[Route("api/node")]
public class NodeController(IMarzbanApiClient client, INodeProvisioningService service, INodeService nodeService)
    : ApiBaseController
{
    [HttpPost("login")]
    [EndpointName("get-marzban-token")]
    [EndpointSummary("Authenticates with the Marzban panel.")]
    [ProducesResponseType(typeof(ApiResult<MarzbanLoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetToken([FromBody] MarzbanLoginRequest request)
    {
        return Ok(await client.LoginAsync(request));
    }

    [HttpPost]
    [EndpointName("create-node")]
    [EndpointSummary("creates a new node server in marzban panel.")]
    [SuperAdminAuthorize]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> CreateNode([FromBody] MarzbanNodeCreateRequest request)
    {
        return Ok(await client.AddNodeAsync(request));
    }

    [HttpPost("initiate-node")]
    [EndpointName("initiate-node")]
    [EndpointSummary("initiates a new node")]
    [EndpointDescription("initiate a new node for marzban panel of user")]
    [ProducesResponseType(typeof(ApiResult<InstanceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InstanceDto>> InitiateNodePurchase([FromBody] InitiateRequestDto request)
    {
        var newInstance = await service.ProvisionAsync(request.NodeId, request.PanelId);
        return Ok(newInstance);
    }

    [HttpPost("add-node")]
    [EndpointName("add-node")]
    [EndpointSummary("adds a new node to the node servers in database")]
    [SuperAdminAuthorize]
    [ProducesResponseType(typeof(ApiResult<NodeDto>), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<NodeDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<NodeDto>> CreateNodeServer([FromBody] NodeCreateDto node)
    {
        return Ok(await nodeService.CreateNodeAsync(node));
    }

    [HttpGet("nodes")]
    [EndpointName("all-nodes")]
    [EndpointSummary("get all node servers")]
    [ProducesResponseType(typeof(ApiResult<List<NodeDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<NodeDto>>> GetNodeServers()
    {
        return Ok(await nodeService.GetNodesAsync());
    }

    [HttpGet("nodes/{nodeId:long}")]
    [EndpointName("node")]
    [EndpointSummary("get a node servers")]
    [ProducesResponseType(typeof(ApiResult<NodeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NodeDto>> GetNodeServer([FromRoute] long nodeId)
    {
        return Ok(await nodeService.GetNodeByIdAsync(nodeId));
    }

    [HttpPost("setting")]
    [EndpointName("setting")]
    [EndpointSummary("get settings for a node")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<MarzbanNodeGetSettingResponse>>> GetNodeSetting(
        [FromBody] MarzbanNodeGetSettingRequest request)
    {
        return Ok(await client.GetNodeSettingAsync(request));
    }

    [HttpDelete("{nodeId:long}")]
    [EndpointName("delete-node")]
    [EndpointSummary("Deletes a node and all related Marzban nodes and instances")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteNode([FromRoute] long nodeId)
    {
        try
        {
            await nodeService.DeleteNodeAsync(nodeId);
            return Ok(new ApiResult(
                true,
                Domain.Common.ApiResultStatusCode.Success,
                null,
                "Node and all related instances deleted."
            ));
        }
        catch (Domain.Exceptions.NotFoundException ex)
        {
            return NotFound((ApiResult)ex);
        }
        catch (Domain.Exceptions.AppException ex)
        {
            return BadRequest((ApiResult)ex);
        }
        catch (Exception ex)
        {
            return BadRequest((ApiResult)ex);
        }
    }

    [HttpGet("admin/nodes")]
    [SuperAdminAuthorize]
    [EndpointName("admin-nodes-health")]
    [EndpointSummary("Gets all nodes with provisioning / agent health data for admins")]
    public async Task<ActionResult<List<NodeDto>>> GetAdminNodes()
    {
        return Ok(await nodeService.GetAllAdminNodesAsync());
    }
}