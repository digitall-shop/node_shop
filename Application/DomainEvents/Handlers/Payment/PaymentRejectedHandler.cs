using Application.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Application.Statics;
using Domain.Events.DomainEvents.Events.Payment;

namespace Application.DomainEvents.Handlers.Payment;

public class PaymentRejectedHandler(
    ILogger<PaymentRejectedHandler> logger,
    IPaymentService paymentService,
    IUserService userService,
    TelegramBotClient botClient
) : INotificationHandler<PaymentRejectedEvent>
{
    public async Task Handle(PaymentRejectedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var request = notification.PaymentRequest;
            var user = await userService.GetUserByIdAsync(request.UserId);

            var reason = string.IsNullOrWhiteSpace(request.Description) ? "—" : request.Description;

            var msg =
                $"❌ پرداخت شما رد شد.\n" +
                $"💰 مبلغ: {request.Amount:N0} تومان\n" +
                $"🆔 شناسه: `{request.Id}`\n" +
                $"📝 دلیل: {reason}";

            await botClient.SendMessage(
                chatId: user.Id,
                text: msg,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken
            );

            logger.PaymentRejectedUserNotified(request.Id, user.Id);
        }
        catch (Exception ex)
        {
            logger.PaymentNotifyErrorUser(notification.PaymentRequest.Id, 0, ex);
        }
    }
}