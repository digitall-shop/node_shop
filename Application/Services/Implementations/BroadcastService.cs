using Application.Services.Interfaces;
using Application.Statics;
using Domain.Contract;
using Domain.DTOs.Broadcast;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace Application.Services.Implementations;

public class BroadcastService(
    IAsyncRepository<User, long> userRepo,
    TelegramBotClient botClient,
    ILogger<IBroadcastService> logger
) : IBroadcastService
{
    private const int BatchSize = 25;
    private const int PauseBetweenBatchesMs = 1800;

    public async Task<BroadcastResultDto> SendToAllAsync(BroadcastCreateDto send)
    {
        if (string.IsNullOrWhiteSpace(send.Text))
            throw new ArgumentException("Message text cannot be empty.", nameof(send.Text));

        logger.BroadcastStarted(send.ParseMode);

        var query = userRepo.GetQuery().AsNoTracking();

        if (send.UserIds is { Length: > 0 })
            query = query.Where(u => !send.UserIds.Contains(u.Id));

        query = query.OrderBy(u => u.Id);

        var result = new BroadcastResultDto();
        var sentInBatch = 0;

        await foreach (var user in query.AsAsyncEnumerable())
        {
            result.TotalTargets++;
            const string detial = "پیام جدید از طرف پشتیبانی :\n\n";

            try
            {
                await botClient.SendMessage(
                    chatId: user.Id,
                    text: detial + send.Text,
                    parseMode: ToParseMode(send.ParseMode),
                    cancellationToken: CancellationToken.None
                );

                result.Sent++;
                sentInBatch++;
            }
            catch (ApiRequestException ex)
            {
                result.Failed++;
                result.FailedUserIds.Add(user.Id);
                logger.BroadcastWarnUser(user.Id, ex.Message);
            }
            catch (Exception ex)
            {
                result.Failed++;
                result.FailedUserIds.Add(user.Id);
                logger.BroadcastErrorUser(user.Id, ex);
            }

            if (sentInBatch >= BatchSize)
            {
                await Task.Delay(PauseBetweenBatchesMs);
                sentInBatch = 0;
            }
        }

        logger.BroadcastFinished(result.TotalTargets, result.Sent, result.Failed);

        return result;
    }

    public async Task<bool> SendToUserAsync(DirectMessageDto send, long userId)
    {
        if (string.IsNullOrWhiteSpace(send.Text))
            throw new ArgumentException("Message text cannot be empty.", nameof(send.Text));

        var user = await userRepo.GetByIdAsync(userId);
        if (user is null)
        {
            logger.DirectMessageResult(userId, success: false, note: "User not found");
            return false;
        }

        try
        {
            await botClient.SendMessage(
                chatId: user.Id,
                text: send.Text,
                parseMode: ToParseMode(send.ParseMode),
                cancellationToken: CancellationToken.None
            );

            logger.DirectMessageResult(userId, success: true);
            return true;
        }
        catch (ApiRequestException ex)
        {
            logger.DirectMessageResult(userId, success: false, note: ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            logger.DirectMessageError(userId, ex);
            return false;
        }
    }

    private static ParseMode ToParseMode(string? mode)
        => mode?.Equals("HTML", StringComparison.OrdinalIgnoreCase) == true
            ? ParseMode.Html
            : ParseMode.Markdown;
}