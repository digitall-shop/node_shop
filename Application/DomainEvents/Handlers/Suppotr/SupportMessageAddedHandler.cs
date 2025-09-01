using Application.DomainEvents.Events.Support;
using Application.Services.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Application.Statics;

namespace Application.DomainEvents.Handlers.Suppotr;

public class SupportMessageAddedHandler(
    ILogger<SupportMessageAddedHandler> logger,
    ISupportService supportService,
    IUserService userService,
    Domain.Contract.IAsyncRepository<SupportMessage, long> messageRepo,
    TelegramBotClient botClient
) : INotificationHandler<SupportMessageAddedEvent>
{
    public async Task Handle(SupportMessageAddedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var ticket  = await supportService.GetTicketAsync(notification.TicketId);
            var message = await messageRepo.GetByIdAsync(notification.MessageId);

            if (message == null)
            {
                logger.LogWarning("Support message {MessageId} not found for ticket {TicketId}",
                    notification.MessageId, notification.TicketId);
                return;
            }

            if (notification.IsFromAdmin)
            {
                // notify user
                var user = await userService.GetUserByIdAsync(ticket.UserId);

                var text =
                    $"✉️ *پاسخ جدید از پشتیبانی*\n\n" +
                    $"🆔 تیکت: `{ticket.Id}`\n" +
                    $"💬 پیام: {message.Text}";

                try
                {
                    await botClient.SendMessage(
                        chatId: user.Id,
                        text: text,
                        parseMode: ParseMode.Markdown,
                        cancellationToken: cancellationToken
                    );
                    logger.SupportNotifyUser(ticket.Id, user.Id, message.Id);
                }
                catch (Exception ex)
                {
                    logger.SupportNotifyErrorUser(ticket.Id, user.Id, ex);
                }
            }
            else
            {
                // notify admins (assigned admin, otherwise all super admins)
                var text =
                    $"👤 *پیام جدید از کاربر در تیکت*\n\n" +
                    $"🆔 تیکت: `{ticket.Id}`\n" +
                    $"💬 پیام: {message.Text}";

                if (ticket.AssignedAdminId is { } assignedId)
                {
                    try
                    {
                        var admin = await userService.GetUserByIdAsync(assignedId);
                        await botClient.SendMessage(
                            chatId: admin.Id,
                            text: text,
                            parseMode: ParseMode.Markdown,
                            cancellationToken: cancellationToken
                        );
                        logger.SupportNotifyAdmin(ticket.Id, admin.Id, message.Id);
                    }
                    catch (Exception ex)
                    {
                        logger.SupportNotifyErrorAdmin(ticket.Id, assignedId, ex);
                    }
                }
                else
                {
                    var admins = await userService.GetAllSuperAdminAsync();
                    if (admins.Count == 0)
                    {
                        logger.LogWarning("No admins to notify for user message in ticket {TicketId}", ticket.Id);
                        return;
                    }

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
                            logger.SupportNotifyAdmin(ticket.Id, admin.Id, message.Id);
                        }
                        catch (Exception ex)
                        {
                            logger.SupportNotifyErrorAdmin(ticket.Id, admin.Id, ex);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to dispatch support message notification. Ticket={TicketId}, Message={MessageId}",
                notification.TicketId, notification.MessageId);
        }
    }
}
