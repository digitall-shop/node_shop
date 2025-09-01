using Application.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Application.Statics;
using Domain.Events.DomainEvents.Events.Payment;

namespace Application.DomainEvents.Handlers.Payment;

public class PaymentApprovedHandler(
    ILogger<PaymentApprovedHandler> logger,
    IUserService userService,
    TelegramBotClient botClient
) : INotificationHandler<PaymentApprovedEvent>
{
    public async Task Handle(PaymentApprovedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var request =notification.PaymentRequest;
            var user = await userService.GetUserByIdAsync(request.UserId);

            var msg =
                $"✅ پرداخت شما تایید شد.\n" +
                $"💰 مبلغ: {request.Amount:N0} تومان\n" +
                $"🆔 شناسه: `{request.Id}`\n" +
                $"💳 موجودی فعلی: {user.Balance:N0} تومان";

            await botClient.SendMessage(
                chatId: user.Id,
                text: msg,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken
            );

            logger.PaymentApprovedUserNotified(request.Id, user.Id);
        }
        catch (Exception ex)
        {
            logger.PaymentNotifyErrorUser(notification.PaymentRequest.Id, 0, ex);
        }
    }
}