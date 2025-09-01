using Application.Builder;
using Application.Services.Interfaces;
using Domain.DTOs.Auth;
using Domain.DTOs.User;
using Domain.Enumes.User;
using Domain.Enumes.Transaction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Handler;

public class TelegramBotService(
    TelegramBotClient client,
    ILogger<TelegramBotService> logger,
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration)
    : BackgroundService
{
    private sealed record PendingReject(long PaymentId, long ChatId, int MessageId, bool IsPhotoMessage);

    private static readonly Dictionary<long, PendingReject> PendingRejections = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = [],
        };

        var me = await client.GetMe(stoppingToken);
        logger.LogInformation("🤖 Bot {Username} started!", me.Username);

        client.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            stoppingToken
        );

        logger.LogInformation("🤖 Bot {Username} is listening for messages...", me.Username);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.CallbackQuery)
        {
            await HandleCallbackQueryAsync(botClient, update.CallbackQuery, cancellationToken);
            return;
        }

        if (update is { Type: UpdateType.Message, Message: { } m })
        {
            if (m.MessageThreadId is { } threadId)
            {
                logger.LogInformation("Topic detected: Title={Title}, ThreadId={ThreadId}, ChatId={ChatId}",
                    m.Chat.Title, threadId, m.Chat.Id);
            }

            if (m is { ForumTopicCreated: not null, MessageThreadId: { } createdId })
            {
                logger.LogInformation("Forum topic created: Name={Name}, ThreadId={ThreadId}",
                    m.ForumTopicCreated.Name, createdId);
            }
        }

        if (update.Message is not { } message)
        {
            logger.LogDebug("Received update is not a message or message is null.");
            return;
        }

        if (message.Text is not { } messageText)
        {
            logger.LogDebug("Received message has no text.");
            return;
        }

        if (PendingRejections.TryGetValue(message.Chat.Id, out var pending))
        {
            var reason = (message.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(reason))
            {
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "⚠️ دلیل نمی‌تواند خالی باشد. لطفاً دوباره ارسال کنید.",
                    cancellationToken: cancellationToken
                );
                return;
            }

            await using var scope = scopeFactory.CreateAsyncScope();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

            try
            {
                var pr = await paymentService.GetPaymentRequestByIdForAdminAsync(pending.PaymentId);
                if (pr.Status != PaymentRequestStatus.Submitted)
                {
                    PendingRejections.Remove(message.Chat.Id);
                    await botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: $"⛔️ امکان رد نیست. وضعیت فعلی: {pr.Status}",
                        cancellationToken: cancellationToken
                    );
                    return;
                }

                await paymentService.RejectPaymentAsync(new() { Description = reason }, pending.PaymentId);

                await botClient.EditMessageReplyMarkup(
                    chatId: pending.ChatId,
                    messageId: pending.MessageId,
                    replyMarkup: null,
                    cancellationToken: cancellationToken
                );

                await botClient.SendMessage(
                    chatId: pending.ChatId,
                    text: $"❌ پرداخت {pending.PaymentId} به دلیل «{reason}» رد شد.",
                    parseMode: ParseMode.Markdown,
                    cancellationToken: cancellationToken
                );

                PendingRejections.Remove(message.Chat.Id);
                return;
            }
            catch (Exception ex)
            {
                PendingRejections.Remove(message.Chat.Id);
                logger.LogError(ex, "RejectPayment failed for PaymentId={PaymentId}", pending.PaymentId);

                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "❗️خطا در رد پرداخت. لطفاً دوباره تلاش کنید.",
                    cancellationToken: cancellationToken
                );
                return;
            }
        }

        logger.LogInformation("Received message '{MessageText}' from chat ID: {ChatId}",
            messageText, message.Chat.Id);

        if (messageText.StartsWith("/start"))
        {
            using var topic = LogContext.PushProperty("Topic", "User");
            using var uid = LogContext.PushProperty("UserId", message.Chat.Id);
            var displayName = message.Chat.Username ?? $"{message.Chat.FirstName} {message.Chat.LastName}".Trim();
            logger.LogInformation("Bot started by user. ChatId={UserId}, Username={Username}", message.Chat.Id,
                displayName);

            await using var scope = scopeFactory.CreateAsyncScope();
            
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

                if (message.From == null) return;

                var create = new UserCreateDto
                {
                    ChatId = message.Chat.Id,
                    FirstName = message.Chat.FirstName,
                    LastName = message.Chat.LastName,
                    UserName = message.Chat.Username,
                };

                var user = await userService.GetOrCreateUserAsync(create);

                var tokenModel = new LoginDto
                {
                    UserName = user.UserName,
                    Password = user.UserName
                };

                var token = await authService.LoginAsync(tokenModel);
                logger.LogInformation("Generated JWT Token for user {Username}: {Token}", user.UserName, token);

                logger.LogInformation("User processed and/or saved: Id={UserId}, Username={Username}", create.ChatId,
                    user.UserName);
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text:
                    $"سلام{user.FirstName ?? user.UserName ?? "کاربر عزیز"}! خوش آمدید به فروشگاه ما. با کلیک روی دکمه زیر وارد مینی اپ شوید:",
                    replyMarkup: TelegramKeyboardBuilder.BuildOpenShopKeyboard(token.Token),
                    cancellationToken: cancellationToken);
            }
        }
        else
        {
            await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: "متوجه پیامتون نشدم. لطفاً از دستور /start استفاده کنید.",
                cancellationToken: cancellationToken);
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };
        logger.LogError("Error in Telegram bot: {ErrorMessage}", errorMessage);
        return Task.CompletedTask;
    }

    private async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery? callbackQuery,
        CancellationToken cancellationToken)
    {
        if (callbackQuery.Data is not { } data) return;

        await using var scope = scopeFactory.CreateAsyncScope();
        var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
        var userService   = scope.ServiceProvider.GetRequiredService<IUserService>();

        var dataParts = data.Split(':');
        var action    = dataParts[0];
        var chatId    = callbackQuery.Message!.Chat.Id;
        var messageId = callbackQuery.Message.MessageId;

        switch (action)
        {
            case "grant_payment":
            {
                var methodStr    = dataParts[1];
                var targetUserId = long.Parse(dataParts[2]);

                var adminUser = await userService.GetUserByIdAsync(callbackQuery.From.Id);
                if (!adminUser.IsSuperAdmin)
                {
                    await botClient.AnswerCallbackQuery(
                        callbackQuery.Id,
                        text: "Only super admins can perform this action.",
                        showAlert: true,
                        url: null,
                        cancellationToken: cancellationToken
                    );
                    break;
                }

                var method = Enum.TryParse<PaymentMethod>(methodStr, out var m)
                    ? m
                    : PaymentMethod.CardToCard;

                var access = method == PaymentMethod.CardToCard
                    ? PaymentMethodAccess.CardToCard
                    : PaymentMethodAccess.Plisio;

                await userService.GrantPaymentMethodAsync(targetUserId, access);

                await botClient.AnswerCallbackQuery(
                    callbackQuery.Id,
                    text: "Payment method activated for this user.",
                    showAlert: false,
                    url: null,
                    cancellationToken: cancellationToken
                );

                var methodFa = method == PaymentMethod.CardToCard ? "کارت به کارت" : "درگاه Plisio";
                await botClient.SendMessage(
                    chatId: targetUserId,
                    text: $"✅ روش پرداخت *{methodFa}* برای حساب شما فعال شد. حالا می‌توانید از طریق مینی‌اپ اقدام کنید.",
                    parseMode: ParseMode.Markdown,
                    cancellationToken: cancellationToken
                );

                if (callbackQuery.Message != null)
                {
                    await botClient.EditMessageText(
                        chatId: callbackQuery.Message.Chat.Id,
                        messageId: callbackQuery.Message.MessageId,
                        text: (callbackQuery.Message.Text ?? callbackQuery.Message.Caption ?? string.Empty) + "\n\n— *فعال‌سازی انجام شد ✅*",
                        parseMode: ParseMode.Markdown,
                        replyMarkup: null,
                        cancellationToken: cancellationToken
                    );
                }

                break;
            }

            case "approve_payment":
            {
                var paymentId = long.Parse(dataParts[1]);

                var pr = await paymentService.GetPaymentRequestByIdForAdminAsync(paymentId);
                if (pr.Status != PaymentRequestStatus.Submitted)
                {
                    await botClient.AnswerCallbackQuery(
                        callbackQuery.Id,
                        text: $"⛔️ امکان تأیید نیست. وضعیت فعلی: {pr.Status}",
                        showAlert: true,
                        url: null,
                        cancellationToken: cancellationToken
                    );
                    break;
                }

                try
                {
                    await paymentService.ApprovePaymentAsync(paymentId);
                    var hasPhoto       = callbackQuery.Message!.Photo?.Length > 0;
                    var approvedSuffix = "\n\n—\n✅ *پرداخت تایید شد.*";

                    if (hasPhoto)
                    {
                        var currentCaption = callbackQuery.Message!.Caption ?? string.Empty;
                        await botClient.EditMessageCaption(
                            chatId: chatId,
                            messageId: messageId,
                            caption: currentCaption + approvedSuffix,
                            parseMode: ParseMode.Markdown,
                            replyMarkup: null,
                            cancellationToken: cancellationToken
                        );
                    }
                    else
                    {
                        var currentText = callbackQuery.Message!.Text ?? string.Empty;
                        await botClient.EditMessageText(
                            chatId: chatId,
                            messageId: messageId,
                            text: currentText + approvedSuffix,
                            parseMode: ParseMode.Markdown,
                            replyMarkup: null,
                            cancellationToken: cancellationToken
                        );
                    }

                    await botClient.AnswerCallbackQuery(
                        callbackQuery.Id,
                        text: "✅ پرداخت با موفقیت تایید شد.",
                        showAlert: false,
                        url: null,
                        cancellationToken: cancellationToken
                    );
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "ApprovePayment failed for PaymentId={PaymentId}", paymentId);
                    await botClient.AnswerCallbackQuery(
                        callbackQuery.Id,
                        text: "❗️خطای غیرمنتظره هنگام تایید پرداخت.",
                        showAlert: true,
                        url: null,
                        cancellationToken: cancellationToken
                    );
                }

                break;
            }

            case "reject_payment":
            {
                var paymentId = long.Parse(dataParts[1]);

                var pr = await paymentService.GetPaymentRequestByIdForAdminAsync(paymentId);
                if (pr.Status != PaymentRequestStatus.Submitted)
                {
                    await botClient.AnswerCallbackQuery(
                        callbackQuery.Id,
                        text: $"⛔️ امکان رد نیست. وضعیت فعلی: {pr.Status}",
                        showAlert: true,
                        url: null,
                        cancellationToken: cancellationToken
                    );
                    break;
                }

                var hasPhoto = callbackQuery.Message!.Photo?.Length > 0;

                PendingRejections[chatId] = new PendingReject(
                    PaymentId: paymentId,
                    ChatId: chatId,
                    MessageId: messageId,
                    IsPhotoMessage: hasPhoto
                );

                await botClient.AnswerCallbackQuery(
                    callbackQuery.Id,
                    text: "✍️ لطفاً دلیل رد را بنویسید…",
                    showAlert: false,
                    url: null,
                    cancellationToken: cancellationToken
                );

                await botClient.SendMessage(
                    chatId: chatId,
                    text: "❌ لطفاً دلیل رد کردن این پرداخت را در یک پیام ارسال کنید:",
                    cancellationToken: cancellationToken
                );

                break;
            }

            case "view_payment":
            {
                var paymentId = long.Parse(dataParts[1]);

                var request = await paymentService.GetPaymentRequestByIdForAdminAsync(paymentId);
                var targetUser = await userService.GetUserByIdAsync(request.UserId);

                var detailsText =
                    $"🔔 *درخواست پرداخت جدید*\n\n" +
                    $"👤 **کاربر:** {targetUser.FirstName} {targetUser.LastName}\n" +
                    $"🆔 **شناسه کاربر:** `{targetUser.Id}`\n" +
                    $"💰 **مبلغ:** {request.Amount:N0} تومان\n" +
                    $"🚦 **وضعیت:** {request.Status}\n" +
                    $"📅 **تاریخ ثبت:** {request.CreateDate:g}\n" +
                    $"🔗 **شناسه درگاه:** `{request.GatewayTransactionId}`\n\n" +
                    $"لطفاً بررسی و اقدام نمایید.";

                var hostAddress = configuration["HOST_ADDRESS"];
                var photoUrl    = $"{hostAddress}{request.ReceiptImageUrl}";

                await botClient.SendPhoto(
                    chatId: chatId,
                    photo: photoUrl,
                    caption: detailsText,
                    parseMode: ParseMode.Markdown,
                    cancellationToken: cancellationToken
                );
                break;
            }

            default:
                logger.LogWarning("Received unknown callback query action: {Action}", action);
                break;
        }
        await botClient.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: cancellationToken);
    }
}
