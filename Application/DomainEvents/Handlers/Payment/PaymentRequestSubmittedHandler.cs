using Application.Builder;
using Application.Infrastructure;
using Application.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Application.Statics;
using Domain.Events.DomainEvents.Events.Payment;

namespace Application.DomainEvents.Handlers.Payment;

public class PaymentRequestSubmittedHandler(
    ILogger<PaymentRequestSubmittedHandler> logger,
    IUserService userService,
    TelegramBotClient botClient,
    IFileService fileService
) : INotificationHandler<PaymentRequestSubmittedEvent>
{
    public async Task Handle(PaymentRequestSubmittedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var pr = notification.PaymentRequest;
            
            var user = await userService.GetUserByIdAsync(pr.UserId);
            var admins = await userService.GetAllSuperAdminAsync();

            if (!admins.Any())
            {
                logger.LogWarning("No super admins found to send notification to. PaymentId={PaymentId}", pr.Id);
                return;
            }

            var fullReceiptUrl = fileService.GetFullUrl(pr.ReceiptImageUrl);
            var detailsText =
                $"🔔 *پرداخت جدید*\n\n" +
                $"👤 **کاربر:** {user.FirstName} {user.LastName}\n" +
                $"🆔 **شناسه کاربر:** `{user.Id}`\n" +
                $"💰 **مبلغ:** {pr.Amount:N0} تومان\n" +
                $"🚦 **وضعیت:** {pr.Status}\n" +
                $"📅 **تاریخ ثبت:** {pr.CreateDate:g}\n" +
                $"🔗 **شناسه درگاه:** `{pr.GatewayTransactionId}`\n\n" +
                $"لطفاً بررسی و اقدام نمایید.";

            foreach (var admin in admins)
            {
                try
                {
                    await botClient.SendPhoto(
                        chatId: admin.Id,
                        photo: fullReceiptUrl,
                        caption: detailsText,
                        parseMode: ParseMode.Markdown,
                        replyMarkup: TelegramKeyboardBuilder.BuildPaymentResult(pr.Id),
                        cancellationToken: cancellationToken
                    );
                    logger.PaymentSubmittedAdminNotified(pr.Id, admin.Id);
                }
                catch (Exception ex)
                {
                    logger.PaymentNotifyErrorAdmin(pr.Id, admin.Id, ex);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send payment submission notification. PaymentId={PaymentId}",
                notification.PaymentRequest.Id);
        }
    }
}