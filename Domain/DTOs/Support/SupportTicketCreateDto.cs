namespace Domain.DTOs.Support;

public class SupportTicketCreateDto
{
    public string? Subject { get; set; } = null;
    public string? FirstMessage { get; set; }
}