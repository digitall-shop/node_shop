
namespace Application.Logging.Options
{
    public class TelegramTopicLoggerOptions
    {
        public string BotToken { get; set; } = "";
        public long ChatId { get; set; }       
        public int DefaultTopicId { get; set; } = 0;  
        public string TopicPropertyName { get; set; } = "Topic";
        public int MaxLength { get; set; } = 4000;
        public bool SplitLongMessages { get; set; } = true;

        public Dictionary<string, int> Topics { get; set; } = new();
    }
}