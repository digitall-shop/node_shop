using Application.Builder;
using Application.Options;
using Application.Services.Interfaces;
using Domain.Contract;
using Domain.Entities;
using Domain.Enumes.Container;
using Domain.Enumes.Transaction;
using Domain.Events.DomainEvents.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Application.DomainEvents;

public class LowBalanceOnBalanceChangedHandler(
    ILogger<LowBalanceOnBalanceChangedHandler> logger,
    IAsyncRepository<Transaction, long> txRepo,
    IAsyncRepository<Instance, long> instanceRepo,
    IAsyncRepository<User, long> userRepo,
    IAuthService authService,
    TelegramBotClient botClient,
    IOptions<LowBalanceAlertOptions> options
) : INotificationHandler<UserBalanceChangedEvent>
{
    public async Task Handle(UserBalanceChangedEvent notification, CancellationToken ct)
    {
        var o = options.Value;
        if (!o.Enabled) return;

        var user = await userRepo.GetByIdAsync(notification.UserId);
        if (user is null) return;

        var available = user.Balance + user.Credit;

        var lastTopUp = await txRepo.GetQuery()
            .Where(t => t.UserId == user.Id
                        && t.Type == TransactionType.Credit
                        && t.Reason == TransactionReason.TopUp)
            .OrderByDescending(t => t.Timestamp)
            .Select(t => (decimal?)t.Amount)
            .FirstOrDefaultAsync(ct);

        if (lastTopUp is null)
        {
            return;
        }

        decimal dynThreshold = lastTopUp.Value * o.PercentThreshold;       
        decimal dynReset     = lastTopUp.Value * o.ResetPercent;         
        if (o.MinAbsThreshold > 0 && dynThreshold < o.MinAbsThreshold)
            dynThreshold = o.MinAbsThreshold;

        decimal current = available;

        if (user.LowBalanceNotified.HasValue && current >= dynReset)
        {
            user.LowBalanceNotified = false;
            await userRepo.UpdateEntity(user);
            await userRepo.SaveChangesAsync();
            logger.LogInformation("Low-balance flag reset for UserId={UserId}. Bal={Bal}, Reset={Reset}",
                user.Id, current, dynReset);
            return;
        }

        var activeCount = await instanceRepo.GetQuery()
            .CountAsync(i => i.UserId == user.Id && i.Status == ContainerProvisionStatus.Running, ct);

        if (activeCount <= 0) return; 

        if (!user.LowBalanceNotified.HasValue && current < dynThreshold)
        {
            var uname = string.IsNullOrWhiteSpace(user.UserName) ? user.Id.ToString() : user.UserName;
            var token = await authService.LoginAsync(new Domain.DTOs.Auth.LoginDto
            {
                UserName = uname,
                Password = uname
            });

            var warnText =
                $"⚠️ *هشدار کاهش موجودی*\n\n" +
                $"⛽️ موجودی فعلی شما به آستانه تعلیق سرویس ها  رسیده.\n" +
                $"🧾 آخرین پرداخت: `{lastTopUp.Value:N0}` تومان\n" +
                $"🔻 آستانه فعلی: `{dynThreshold:N0}` تومان\n\n" +
                $"❗️ در صورت عدم شارژ، سرویس‌های فعال شما ممکن است *متوقف* شوند.\n\n" +
                $"برای جلوگیری از قطعی، از دکمه زیر استفاده کنید:";

            await botClient.SendMessage(
                chatId: user.Id,
                text: warnText,
                parseMode: ParseMode.Markdown,
                replyMarkup: TelegramKeyboardBuilder.BuildOpenShopKeyboard(token.Token),
                cancellationToken: ct
            );

            user.LowBalanceNotified = true;
            await userRepo.UpdateEntity(user);
            await userRepo.SaveChangesAsync();

            logger.LogWarning("Low-balance alert sent. UserId={UserId} Bal={Bal} Threshold={Threshold}",
                user.Id, current, dynThreshold);
        }
    }
}