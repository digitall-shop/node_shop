using Domain.Common;
using Domain.DTOs.Support;

namespace Application.Services.Interfaces;

public interface ISupportService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="create"></param>
    /// <returns></returns>
    Task<long> CreateTicketAsync(SupportTicketCreateDto create);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    Task<SupportTicketDto> GetTicketAsync(long ticketId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="create"></param>
    /// <returns></returns>
    Task<SupportMessageDto> AddMessageAsync(long ticketId, SupportMessageCreateDto create);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    Task<bool> CloseTicketAsync(long ticketId);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="adminId"></param>
    /// <returns></returns>
    Task<bool> AssignAdminAsync(long ticketId, long adminId);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ticketId"></param>
    /// <param name="forAdmin"></param>
    /// <returns></returns>
    Task MarkAsReadAsync(long ticketId, bool forAdmin); 
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    Task<BasePaging<SupportTicketDto>> GetTicketsForAdminAsync(SupportTicketFilterDto filter);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    Task<BasePaging<SupportTicketDto>> GetTicketsForUserAsync(SupportTicketFilterDto filter);
}