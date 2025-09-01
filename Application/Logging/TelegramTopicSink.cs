using System.Text;
using System.Threading.Channels;
using Application.Logging.Options;
using Serilog.Core;
using Serilog.Events;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Application.Logging
{
    public class TelegramTopicSink : ILogEventSink, IDisposable
    {
        private readonly TelegramTopicLoggerOptions _options;
        private readonly ITelegramBotClient _bot;
        private readonly Channel<(int? topicId, string text)> _queue;
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _senderTask;

        public TelegramTopicSink(TelegramTopicLoggerOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.BotToken))
                throw new ArgumentException("BotToken is required for TelegramTopicSink");

            _bot = new TelegramBotClient(options.BotToken);
            _queue = Channel.CreateBounded<(int? topicId, string text)>(capacity: 500);

            _senderTask = Task.Run(SenderLoopAsync);
        }

        public void Emit(LogEvent logEvent)
        {
            try
            {
                var topicId = ResolveTopicId(logEvent);
                foreach (var part in FormatMessageParts(logEvent))
                {
                    _queue.Writer.TryWrite((topicId, part));
                }
            }
            catch
            {
               
            }
        }

        private int? ResolveTopicId(LogEvent e)
        {
            if (e.Properties.TryGetValue(_options.TopicPropertyName, out var prop))
            {
                var key = prop.ToString().Trim('"');
                if (_options.Topics.TryGetValue(key, out var t))
                    return t;
            }

            if (e.Properties.TryGetValue("SourceContext", out var sc))
            {
                var ctx = sc.ToString().Trim('"');

                foreach (var kv in _options.Topics)
                {
                    if (ctx.IndexOf(kv.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                        return kv.Value;
                }
            }

            return _options.DefaultTopicId;
        }

        private IEnumerable<string> FormatMessageParts(LogEvent e)
        {
            var sb = new StringBuilder();
            sb.Append($"{LevelEmoji(e.Level)} *{e.Level}* `");
            sb.Append(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine(" UTC`");

            sb.AppendLine(e.RenderMessage());

            if (e.Properties is { Count: > 0 })
            {
                var extras = string.Join(", ",
                    e.Properties.Select(kv => $"{kv.Key}={TrimQuotes(kv.Value?.ToString())}"));
                if (!string.IsNullOrWhiteSpace(extras))
                    sb.AppendLine($"`{Limit(extras, 800)}`");
            }

            // Exception
            if (e.Exception != null)
            {
                var exText = e.Exception.ToString();
                if (exText.Length > 1500) exText = exText[..1500] + " …";
                sb.AppendLine("```");
                sb.AppendLine(exText);
                sb.AppendLine("```");
            }

            var full = sb.ToString();
            if (!_options.SplitLongMessages || full.Length <= _options.MaxLength)
                return [full];

            
            var parts = new List<string>();
            for (int i = 0; i < full.Length; i += _options.MaxLength)
            {
                var chunk = full.Substring(i, Math.Min(_options.MaxLength, full.Length - i));
                parts.Add(chunk);
            }
            return parts;
        }

        private async Task SenderLoopAsync()
        {
            try
            {
                while (await _queue.Reader.WaitToReadAsync(_cts.Token))
                {
                    while (_queue.Reader.TryRead(out var item))
                    {
                        try
                        {
                            await _bot.SendMessage(
                                chatId: _options.ChatId,
                                text: item.text,
                                parseMode: ParseMode.Markdown,
                                messageThreadId: item.topicId,
                                cancellationToken: _cts.Token
                            );
                            await Task.Delay(50, _cts.Token);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private static string LevelEmoji(LogEventLevel l) => l switch
        {
            LogEventLevel.Verbose => "📝",
            LogEventLevel.Debug => "🐛",
            LogEventLevel.Information => "ℹ️",
            LogEventLevel.Warning => "⚠️",
            LogEventLevel.Error => "❌",
            LogEventLevel.Fatal => "🚨",
            _ => "❓"
        };

        private static string TrimQuotes(string? s) => (s ?? "").Trim('"');

        private static string Limit(string s, int max) => s.Length <= max ? s : s[..max] + "…";

        public void Dispose()
        {
            try
            {
                _queue.Writer.TryComplete();
                _cts.Cancel();
                _senderTask.Wait(TimeSpan.FromSeconds(2));
                _cts.Dispose();
            }
            catch { }
        }
    }
}
