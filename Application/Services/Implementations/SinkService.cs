using Domain.Contract;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using Serilog.Events;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Application.Services.Implementations;

public class SinkService(IServiceProvider serviceProvider, string botToken, string chatId)
    : ILogEventSink
{
    public void Emit(LogEvent logEvent)
    {
        try
        {
            if (logEvent.Level < LogEventLevel.Warning)
                return;

            Task.Run(async () =>
            {
                using var scope = serviceProvider.CreateScope();

                var logRepository = scope.ServiceProvider.GetRequiredService<IAsyncRepository<Log, long>>();
                var botClient = new TelegramBotClient(botToken);

                var logEntry = new Log
                {
                    Timestamp = logEvent.Timestamp.UtcDateTime,
                    Level = logEvent.Level.ToString(),
                    Message = logEvent.RenderMessage(),
                    MessageTemplate = logEvent.MessageTemplate.Text,
                    Exception = logEvent.Exception?.ToString(),
                    Properties = logEvent.Properties.Count > 0 ? logEvent.Properties.ToString() : null
                };

                await logRepository.AddEntity(logEntry);
                await logRepository.SaveChangesAsync();

                var emoji = logEvent.Level switch
                {
                    LogEventLevel.Error => "❌",
                    LogEventLevel.Fatal => "🚨",
                    LogEventLevel.Warning => "⚠️",
                    _ => "ℹ️"
                };

                var title = $"*{emoji} {logEvent.Level} | NodeShop API*";
                var message = $"`{logEvent.RenderMessage()}`";

                var telegramMessage = $"{title}\n\n" +
                                      $"*Message:*\n{message}\n\n" +
                                      $"*Timestamp:*\n`{logEvent.Timestamp.ToLocalTime():yyyy-MM-dd HH:mm:ss}`";

                if (logEvent.Exception != null)
                {
                    var exceptionText = logEvent.Exception.ToString();
                    telegramMessage +=
                        $"\n\n*Exception:*\n`{exceptionText.Substring(0, Math.Min(exceptionText.Length, 1000))}`";
                }

                await botClient.SendMessage(
                    chatId: chatId,
                    text: telegramMessage,
                    parseMode: ParseMode.Markdown
                );
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FATAL: Error in custom Serilog Sink: {ex}");
        }
    }
}