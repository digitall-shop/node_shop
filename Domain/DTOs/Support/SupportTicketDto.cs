using Domain.Enumes.Suppotr;

namespace Domain.DTOs.Support;

public class SupportTicketDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string? Subject { get; set; } = null;
    public SupportTicketStatus Status { get; set; }
    public long? AssignedAdminId { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? ModifiedDate { get; set; } 
}