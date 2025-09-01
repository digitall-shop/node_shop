using Application.DomainEvents.Events.Support;
using Application.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Application.Statics;

namespace Application.DomainEvents.Handlers.Suppotr;

public class SupportTicketCreatedHandler(
    ILogger<SupportTicketCreatedHandler> logger,
    ISupportService supportService,
    IUserService userService,
    TelegramBotClient botClient
) : INotificationHandler<SupportTicketCreatedEvent>
{
    public async Task Handle(SupportTicketCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var ticket = await supportService.GetTicketAsync(notification.TicketId);
            var user = await userService.GetUserByIdAsync(ticket.UserId);
            var admins = await userService.GetAllSuperAdminAsync();

            if (admins.Count == 0)
            {
                logger.LogWarning("No super admins to notify for support ticket {TicketId}.", notification.TicketId);
                return;
            }

            var text =
                $"🆕 *تیکت جدید پشتیبانی*\n\n" +
                $"👤 کاربر: {user.FirstName}\n" +
                $"🆔 شناسه کاربر: `{user.Id}`\n" +
                $"📝 موضوع: {ticket.Subject}\n" +
                $"🆔 شناسه تیکت: `{ticket.Id}`\n" +
                $"📌 وضعیت: {ticket.Status}";

            logger.SupportTicketNotifyAdmins(ticket.Id, admins.Count);

            foreach (var admin in admins)
            {
                try
                {
                    await botClient.SendMessage(
                        chatId: admin.Id,
                        text: text,
                        parseMode: ParseMode.Markdown,
                        cancellationToken: cancellationToken
                    );
                    logger.SupportNotifyAdmin(ticket.Id, admin.Id, 0);
                }
                catch (Exception ex)
                {
                    logger.SupportNotifyErrorAdmin(ticket.Id, admin.Id, ex);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to notify admins about new support ticket {TicketId}", notification.TicketId);
        }
    }
}