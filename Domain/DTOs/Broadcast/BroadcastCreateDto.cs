namespace Domain.DTOs.Broadcast;

public class BroadcastCreateDto
{
    public string Text { get; set; } = string.Empty;
    public string ParseMode { get; set; } = "Markdown";

    public bool DisableWebPagePreview { get; set; } = true;
    public long[]? UserIds { get; set; } 
}