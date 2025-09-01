using Api.Filters;
using Api.Securities;
using Application.Services.Interfaces;
using Domain.Common;
using Domain.DTOs.Support;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Tags("Support Endpoint")]
[Route("api/support")]
public class SupportController(ISupportService service) : ApiBaseController
{

    [HttpPost("tickets")]
    [EndpointName("create-ticket")]
    [EndpointSummary("Creates a new support ticket for the current user.")]
    [EndpointDescription("User can create a ticket and (optionally) send the first message.")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<long>> CreateTicket([FromBody] SupportTicketCreateDto create)
        => Ok(await service.CreateTicketAsync(create));

    [HttpGet("tickets/{ticketId:long}")]
    [EndpointName("get-ticket")]
    [EndpointSummary("Gets a single support ticket by id.")]
    [EndpointDescription("Accessible to the ticket owner or admins. Returns the ticket details.")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SupportTicketDto>> GetTicket(long ticketId)
    {
        var ticket = await service.GetTicketAsync(ticketId);
        return Ok(ticket);
    }
         


    [HttpPost("tickets/{ticketId:long}/messages")]
    [EndpointName("add-ticket-message")]
    [EndpointSummary("Adds a new message to the support ticket.")]
    [EndpointDescription("User or admin can add message to a ticket. No file upload for now.")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SupportMessageDto>> AddMessage(long ticketId,
        [FromBody] SupportMessageCreateDto create)
    {
        var message = await service.AddMessageAsync(ticketId, create);
        return Ok(message);
    }
        


    [HttpPost("tickets/{ticketId:long}/close")]
    [EndpointName("close-ticket")]
    [EndpointSummary("Closes an open support ticket.")]
    [EndpointDescription("Owner or admin can close the ticket.")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Close(long ticketId)
    {
       var close= await service.CloseTicketAsync(ticketId);
        return Ok(close);
    }

    [HttpPost("admin/tickets/{ticketId:long}/assign/{adminId:long}")]
    [EndpointName("assign-ticket-to-admin")]
    [EndpointSummary("Assigns a ticket to a specific admin.")]
    [EndpointDescription("Admin-only endpoint to assign responsibility for a ticket.")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Assign(long ticketId, long adminId)
    {
        var response= await service.AssignAdminAsync(ticketId, adminId);
        return Ok(response);
    }

    
    [HttpPost("tickets/{ticketId:long}/read")]
    [EndpointName("mark-ticket-read")]
    [EndpointSummary("Marks unread messages of a ticket as read for user or admin.")]
    [EndpointDescription("Set forAdmin=true to mark for admin; otherwise marks for user.")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> MarkRead(long ticketId, [FromQuery] bool forAdmin = false)
    { await service.MarkAsReadAsync(ticketId, forAdmin); return Ok(); }
    
    [HttpGet("tickets/admin")]
    [EndpointName("list-tickets-admin")]
    [EndpointSummary("List tickets for admin with filtering/paging")]
    [EndpointDescription("Admins can list all tickets using filters (status, assigned admin, subject, date range).")]
    [SuperAdminAuthorize]
    [ProducesResponseType(typeof(ApiResult<BasePaging<SupportTicketDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BasePaging<SupportTicketDto>>> ListForAdmin([FromQuery] SupportTicketFilterDto filter)
        => Ok(await service.GetTicketsForAdminAsync(filter));

    [HttpGet("tickets/my")]
    [EndpointName("list-tickets-user")]
    [EndpointSummary("List my tickets with filtering/paging")]
    [EndpointDescription("Returns only tickets belonging to the current user.")]
    [ProducesResponseType(typeof(ApiResult<BasePaging<SupportTicketDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BasePaging<SupportTicketDto>>> ListForUser([FromQuery] SupportTicketFilterDto filter)
        => Ok(await service.GetTicketsForUserAsync(filter));
}
