using Api.Filters;
using Api.Securities;
using Application.Services.Interfaces;
using Domain.DTOs.Panel;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Tags("Panel Endpoint")]
[Route("api/panel")]
public class PanelController(IPanelService service) : ApiBaseController
{
    [HttpGet("{id:long}")]
    [EndpointName("panel")]
    [EndpointSummary("get panel by id")]
    [EndpointDescription("this for get panel by id")]
    [ProducesResponseType(typeof(ApiResult<PanelDto>), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ApiResult<PanelDto>> GetPanel(long id)
    {
        return Ok(await service.GetPanelAsync(id));
    }

    [HttpGet]
    [EndpointName("panels")]
    [EndpointSummary("list panels")]
    [EndpointDescription("this for get list panels")]
    [SuperAdminAuthorize]
    [ProducesResponseType(typeof(ApiResult<List<PanelDto>>), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ApiResult<List<PanelDto>>> GetAllPanels()
    {
        return Ok(await service.GetAllPanelsAsync());
    }
    
    [HttpGet("user")]
    [EndpointName("user-panels")]
    [EndpointSummary("list user panels")]
    [EndpointDescription("this for get all user list panels")]
    [ProducesResponseType(typeof(ApiResult<List<PanelDto>>), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ApiResult<List<PanelDto>>> GetAllUserPanels()
    {
        return Ok(await service.GetAllUserPanelsAsync());
    }

    [HttpPost]
    [EndpointName("create-panel")]
    [EndpointSummary("create new panel")]
    [EndpointDescription("this for create new panel")]
    [ProducesResponseType(typeof(ApiResult<PanelOverviewDto>), statusCode: StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ApiResult<PanelOverviewDto>> CreatePanel([FromBody] PanelCreateDto panel)
    {
        return Ok(await service.CreatePanelAsync(panel));
    }

    [HttpDelete("{id:long}")]
    [EndpointName("delete-panel")]
    [EndpointSummary("soft delete panel by id")]
    [EndpointDescription("this fro soft delete panel by id")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ApiResult> DeletePanel(long id)
    {
        await service.DeletePanelAsync(id);
        return Ok();
    }
    
    [HttpPatch("{id:long}")] 
    [EndpointName("update-panel")]
    [EndpointSummary("updates an existing panel")]
    [EndpointDescription("user can update a panel that have")]
    [ProducesResponseType(typeof(ApiResult<PanelOverviewDto>), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)] 
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)] 
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ApiResult<PanelOverviewDto>> UpdatePanel([FromRoute] long id,[FromBody] PanelUpdateDto update)
    {
        var updatedPanel = await service.UpdatePanelAsync(update,id);
        return Ok(updatedPanel);
    }
}