using Api.Filters;
using Api.Securities;
using Application.Services.Interfaces;
using Domain.DTOs.Instance;
using Domain.DTOs.Node;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;
[Tags("Instance Endpoint")]
[Route("api/instance")]
public class InstanceController(IInstanceService service) : ApiBaseController
{
    [AllowAnonymous]
    [HttpPost("report")]
    [EndpointName("report")]
    [EndpointSummary("get report from NM")]
    [EndpointDescription("this for reporting instance from NM to this app")]
    [ApiKeyAuthorize]
    [ProducesResponseType(typeof(ApiResult),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SubmitUsageReport([FromBody] UsageReportDto report)
    {
        await service.ProcessUsageReportAsync(report);
        return Ok();
    }

    [HttpGet("all")]
    [EndpointName("instances")]
    [EndpointSummary("get all instances")]
    [EndpointDescription("admin can get all instances")]
    [SuperAdminAuthorize]
    [ProducesResponseType(typeof(ApiResult<List<InstanceDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<InstanceDto>>> GetAllInstances()
    {
        var instances = await service.GetAllInstancesAsync();
        return Ok(instances);
    }
    
    [HttpGet("user/all")]
    [EndpointName("user-instances")]
    [EndpointSummary("get all user instances")]
    [EndpointDescription("user can get all instances")]
    [ProducesResponseType(typeof(ApiResult<List<InstanceDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<InstanceDto>>> GetAllUserInstances()
    {
        var instances = await service.GetAllUserInstancesAsync();
        return Ok(instances);
    }
    
    [HttpGet("panel/{panelId:long}")]
    [EndpointName("get-panel-nodes")]
    [EndpointSummary("get all nodes connected to a specific panel")]
    [EndpointDescription("returns a list of nodes that have at least one instance connected to the specified panel.")]
    [ProducesResponseType(typeof(ApiResult<List<NodeDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<NodeDto>>> GetNodesByPanel([FromRoute] long panelId)
    {
        var nodes = await service.GetNodesByPanelIdAsync(panelId);
        return Ok(nodes);
    }

    [HttpPost("{instanceId:long}/pause")]
    [EndpointName("manual-pause")]
    [EndpointSummary("manual pause the instance")]
    [EndpointDescription("user can do manual pause the instance")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ManualPause([FromRoute] long instanceId)
    {
        await service.ManuallyPauseInstanceAsync(instanceId);
        return Ok();
    }
    
    [HttpPost("{instanceId:long}/unpause")]
    [EndpointName("manual-unpause")]
    [EndpointSummary("manual unpause the instance")]
    [EndpointDescription("user can do manual unpause the instance")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ManualUnPause([FromRoute] long instanceId)
    {
        await service.ManuallyResumeInstanceAsync(instanceId);
        return Ok();
    }

    [HttpDelete("delete/{instanceId:long}")]
    [EndpointName("delete")]
    [EndpointSummary("delete the instance")]
    [EndpointDescription("user can delete the instance")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteInstance([FromRoute] long instanceId)
    {
        await service.DeleteInstanceAsync(instanceId);
        return Ok();
    }
    
}