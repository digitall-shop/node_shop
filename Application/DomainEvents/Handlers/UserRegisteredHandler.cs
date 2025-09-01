using Application.Builder;
using Application.DomainEvents.Events;
using Application.Services.Interfaces;
using Domain.DTOs.Auth;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Application.Statics;
using Domain.Events.DomainEvents.Events;

namespace Application.DomainEvents.Handlers;

public class UserRegisteredHandler(
    ILogger<UserRegisteredHandler> logger,
    IUserService userService,
    TelegramBotClient botClient,
    IAuthService authService
) : INotificationHandler<UserRegisteredEvent>
{
    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userService.GetUserByIdAsync(notification.UserId);
            var superAdmins = await userService.GetAllSuperAdminAsync();

            var fullName = $"{user.FirstName} {user.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(fullName))
                fullName = user.UserName ?? user.Id.ToString();

            logger.UserRegistered(user.Id, user.UserName, fullName);

            if (superAdmins.Count == 0)
            {
                logger.NoSuperAdminsToNotify(user.Id);
                return;
            }

            var usernameLine = !string.IsNullOrWhiteSpace(user.UserName)
                ? $"🪪 *نام‌کاربری:* `@{user.UserName}`\n"
                : string.Empty;

            var message =
                "🟢 *کاربر جدید ربات را استارت کرد*\n\n" +
                $"👤 *نام:* `{fullName}`\n" +
                $"🆔 *شناسه:* `{user.Id}`\n" +
                usernameLine +
                $"🔗 *پروفایل:* [باز کردن چت](tg://user?id={user.Id})\n" +
                $"⏰ *زمان:* `{DateTime.UtcNow:g} (UTC)`";

            foreach (var admin in superAdmins)
            {
                var uname = string.IsNullOrWhiteSpace(admin.UserName) ? admin.Id.ToString() : admin.UserName;

                var token = await authService.LoginAsync(new LoginDto
                {
                    UserName = uname,
                    Password = uname
                });

                try
                {
                    await botClient.SendMessage(
                        chatId: admin.Id,
                        text: message,
                        parseMode: ParseMode.Markdown,
                        replyMarkup: TelegramKeyboardBuilder.BuildOpenShopKeyboard(token.Token),
                        cancellationToken: cancellationToken
                    );

                    logger.NewUserAdminNotifyResult(admin.Id, user.Id, success: true);
                }
                catch (Exception ex)
                {
                    logger.NewUserAdminNotifyResult(admin.Id, user.Id, success: false, error: ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            logger.UserRegisteredHandlerError(notification.UserId, ex);
        }
    }
}
