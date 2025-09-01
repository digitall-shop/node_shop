using Api.Filters;
using Api.Securities;
using Application.Services.Interfaces;
using Domain.DTOs.Broadcast;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Tags("Broadcast Endpoint")]
[Route("api/broadcast")]
[SuperAdminAuthorize]
public class BroadcastController(IBroadcastService service) : ApiBaseController
{
    [HttpPost]
    [EndpointName("broadcast-to-all")]
    [EndpointSummary("Send a message to ALL users")]
    [EndpointDescription("Sends a text message to every user who has started the bot.")]
    [ProducesResponseType(typeof(BroadcastResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BroadcastResultDto>> BroadcastToAll([FromBody] BroadcastCreateDto send)
        => Ok(await service.SendToAllAsync(send));
    
    [HttpPost("user/{userId:long}")]
    [EndpointName("broadcast-to-user")]
    [EndpointSummary("Send a direct message to a specific user")]
    [EndpointDescription("Sends a text message to a single user by UserId.")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> SendToUser([FromBody] DirectMessageDto send,[FromRoute]long userId)
    { 
        var ok = await service.SendToUserAsync(send,userId);
        return Ok(ok);
    }
}