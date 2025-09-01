using Application.Extensiones;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Contract;
using Domain.DTOs.Support;
using Domain.Entities;
using Domain.Enumes.Suppotr;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Statics; 

namespace Application.Services.Implementations;

public class SupportService(
    IAsyncRepository<SupportTicket, long> ticketRepo,
    IAsyncRepository<SupportMessage, long> messageRepo,
    IMapper mapper,
    ILogger<ISupportService> logger,
    IUserContextService userContext,
    IUserService userService
) : ISupportService
{
    public async Task<long> CreateTicketAsync(SupportTicketCreateDto create)
    {
        var ticket = new SupportTicket
        {
            UserId = userContext.UserId,
            Subject = create.Subject,
            Status = SupportTicketStatus.Open
        };

        await ticketRepo.AddEntity(ticket);
        await ticketRepo.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(create.FirstMessage))
        {
            var msg = new SupportMessage
            {
                TicketId = ticket.Id,
                SenderId = userContext.UserId,
                IsFromAdmin = false,
                Text = create.FirstMessage,
                AttachmentPath = null
            };
            await messageRepo.AddEntity(msg);
            await messageRepo.SaveChangesAsync();

            logger.SupportMessageAdded(ticket.Id, msg.Id, isFromAdmin: false);
        }
        ticket.RaiseCreated();
        await ticketRepo.UpdateEntity(ticket);
        await ticketRepo.SaveChangesAsync();
        logger.SupportTicketCreated(ticket.Id, userContext.UserId, ticket.Subject ?? "");

        return ticket.Id;
    }

    public async Task<SupportTicketDto> GetTicketAsync(long ticketId)
    {
        var t = await ticketRepo.GetByIdAsync(ticketId) ?? throw new NotFoundException("Ticket not found");
        if (t.UserId != userContext.UserId && !(await IsAdmin(userContext.UserId)))
            throw new UnauthorizedAccessException("Not allowed");

        return mapper.Map<SupportTicketDto>(t);
    }

    public async Task<BasePaging<SupportTicketDto>> GetTicketsForAdminAsync(SupportTicketFilterDto filter)
    {
        if (!await IsAdmin(userContext.UserId)) throw new UnauthorizedAccessException();

        var (filters, _) = filter.ToRepositoryParams();

        var data = await ticketRepo.GetPagedDataAsync(
            predicate: null,
            filters: filters,
            page: (filter.Skip / filter.Take) + 1,
            take: filter.Take
        );

        var items = mapper.Map<List<SupportTicketDto>>(data.Items);

        return new BasePaging<SupportTicketDto>
        {
            Items = items,
            Page = data.Page,
            Take = data.Take,
            TotalCount = data.TotalCount
        };
    }

    public async Task<BasePaging<SupportTicketDto>> GetTicketsForUserAsync(SupportTicketFilterDto filter)
    {
        var (filters, _) = filter.ToRepositoryParams();

        var data = await ticketRepo.GetPagedDataAsync(
            predicate: t => t.UserId == userContext.UserId,
            filters: filters,
            page: (filter.Skip / filter.Take) + 1,
            take: filter.Take
        );

        var items = mapper.Map<List<SupportTicketDto>>(data.Items);

        return new BasePaging<SupportTicketDto>
        {
            Items = items,
            Page = data.Page,
            Take = data.Take,
            TotalCount = data.TotalCount
        };
    }

    public async Task<SupportMessageDto> AddMessageAsync(long ticketId, SupportMessageCreateDto create)
    {
        var t = await ticketRepo.GetByIdAsync(ticketId) ?? throw new NotFoundException("Ticket not found");
        var isAdmin = await IsAdmin(userContext.UserId);

        if (t.UserId != userContext.UserId && !isAdmin) throw new UnauthorizedAccessException("Not allowed");
        if (t.Status == SupportTicketStatus.Closed) throw new BadRequestException("Ticket closed");

        var msg = new SupportMessage
        {
            TicketId = ticketId,
            SenderId = userContext.UserId,
            IsFromAdmin = isAdmin,
            Text = create.Text,
            AttachmentPath = null
        };

        await messageRepo.AddEntity(msg);
        await messageRepo.SaveChangesAsync(); 

        t.ModifiedDate = DateTime.UtcNow;        
        msg.RaiseAddedEvent();

        await ticketRepo.UpdateEntity(t);
        await ticketRepo.SaveChangesAsync();


        logger.SupportMessageAdded(ticketId, msg.Id, isAdmin);

        return mapper.Map<SupportMessageDto>(msg);
    }

    public async Task<bool> CloseTicketAsync(long ticketId)
    {
        var t = await ticketRepo.GetByIdAsync(ticketId) ?? throw new NotFoundException("Ticket not found");

        var isAdmin = await IsAdmin(userContext.UserId);
        if (t.UserId != userContext.UserId && !isAdmin) throw new UnauthorizedAccessException("Not allowed");

        t.Status = SupportTicketStatus.Closed;
        t.ClosedAt = DateTime.UtcNow;
        t.ModifiedDate = DateTime.UtcNow;

        await ticketRepo.UpdateEntity(t);
        await ticketRepo.SaveChangesAsync();

        logger.SupportTicketClosed(ticketId, userContext.UserId);

        return true;
    }

    public async Task<bool> AssignAdminAsync(long ticketId, long adminId)
    {
        if (!await IsAdmin(userContext.UserId)) throw new UnauthorizedAccessException();

        var t = await ticketRepo.GetByIdAsync(ticketId) ?? throw new NotFoundException("Ticket not found");
        var admin = await userService.GetUserByIdAsync(adminId);
        if (!admin.IsSuperAdmin) throw new BadRequestException("Target user is not admin");

        t.AssignedAdminId = adminId;
        t.ModifiedDate = DateTime.UtcNow;

        await ticketRepo.UpdateEntity(t);
        await ticketRepo.SaveChangesAsync();

        logger.SupportTicketAssigned(ticketId, adminId);
        return true;
    }

    public async Task MarkAsReadAsync(long ticketId, bool forAdmin)
    {
        var t = await ticketRepo.GetByIdAsync(ticketId) ?? throw new NotFoundException("Ticket not found");
        var isAdmin = await IsAdmin(userContext.UserId);
        if (t.UserId != userContext.UserId && !isAdmin) throw new UnauthorizedAccessException("Not allowed");

        var q = messageRepo.GetQuery().Where(m => m.TicketId == ticketId);
        q = forAdmin
            ? q.Where(m => !m.IsReadByAdmin && !m.IsFromAdmin)
            : q.Where(m => !m.IsReadByUser && m.IsFromAdmin);

        var list = await q.ToListAsync();
        foreach (var m in list)
        {
            if (forAdmin) m.IsReadByAdmin = true;
            else m.IsReadByUser = true;

            await messageRepo.UpdateEntity(m);
        }

        await messageRepo.SaveChangesAsync();
    }

    private async Task<bool> IsAdmin(long userId)
    {
        try
        {
            var u = await userService.GetUserByIdAsync(userId);
            return u.IsSuperAdmin;
        }
        catch
        {
            return false;
        }
    }
}