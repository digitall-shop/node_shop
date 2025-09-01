using Application.Logging;
using Application.Logging.Options;
using Serilog;
using Serilog.Events;
using Serilog.Filters;

namespace Application.Extensiones
{
    public static class SerilogTopicExtensions
    {
        public static LoggerConfiguration WriteToTelegramTopics(
            this LoggerConfiguration lc,
            TelegramTopicLoggerOptions opts)
        {
            return lc
                .Filter.ByIncludingOnly(Matching.WithProperty("Topic"))
                .Filter.ByIncludingOnly(Matching.WithProperty("EventType"))
                .WriteTo.Sink(new TelegramTopicSink(opts), restrictedToMinimumLevel: LogEventLevel.Information);
        }
    }
}