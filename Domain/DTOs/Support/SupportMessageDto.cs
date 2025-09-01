namespace Domain.DTOs.Support;

public class SupportMessageDto
{
    public long Id { get; set; }
    public long TicketId { get; set; }
    public long SenderId { get; set; }
    public bool IsFromAdmin { get; set; }
    public string Text { get; set; } = "";
    public string? AttachmentUrl { get; set; }
    public DateTime CreateDate { get; set; }
}