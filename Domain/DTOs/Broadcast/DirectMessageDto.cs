namespace Domain.DTOs.Broadcast;

public class DirectMessageDto
{
    public string Text { get; set; } = string.Empty;

    public string ParseMode { get; set; } = "Markdown";

    public bool DisableWebPagePreview { get; set; } = true;
}