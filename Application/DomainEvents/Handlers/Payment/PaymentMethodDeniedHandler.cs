using Application.Builder;
using Application.Services.Interfaces;
using Domain.Enumes.Transaction;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Application.Statics;
using Domain.Events.DomainEvents.Events.Payment;

namespace Application.DomainEvents.Handlers.Payment;

public class PaymentMethodDeniedHandler(
    ILogger<PaymentMethodDeniedHandler> logger,
    TelegramBotClient botClient,
    IUserService userService
) : INotificationHandler<PaymentMethodDeniedEvent>
{
    public async Task Handle(PaymentMethodDeniedEvent notification, CancellationToken cancellationToken)
    {
        var user = await userService.GetUserByIdAsync(notification.UserId);
        var admins = await userService.GetAllSuperAdminAsync();
        if (admins.Count == 0)
        {
            logger.LogWarning("No super admins to notify.");
            return;
        }

        var methodFa = notification.Method switch
        {
            PaymentMethod.CardToCard => "کارت به کارت",
            PaymentMethod.Plisio => "درگاه Plisio",
            _ => notification.Method.ToString()
        };

        var adminText =
            $"🚫 *تلاش ناموفق برای استفاده از روش پرداخت*\n\n" +
            $"👤 کاربر: {user.FirstName} {user.LastName} (ID: `{user.Id}`)\n" +
            $"روش: *{methodFa}*\n" +
            $"نتیجه: *برای این کاربر فعال نیست*";

        var kays = TelegramKeyboardBuilder.BuildActivatePaymentButton(user.Id, notification.Method);

        int sent = 0;
        foreach (var admin in admins)
        {
            try
            {
                await botClient.SendMessage(
                    chatId: admin.Id,
                    text: adminText,
                    parseMode: ParseMode.Markdown,
                    replyMarkup: kays,
                    cancellationToken: cancellationToken
                );
                sent++;
            }
            catch (Exception ex)
            {
                logger.PaymentNotifyErrorAdmin(0, admin.Id, ex);
            }
        }

        logger.PaymentDeniedAttemptAdminsNotified(user.Id, notification.Method.ToString(), sent);
    }
}