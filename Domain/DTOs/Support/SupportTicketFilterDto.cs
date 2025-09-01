using Domain.Common;
using Domain.Enumes.Suppotr;

namespace Domain.DTOs.Support;

public class SupportTicketFilterDto : PagedDto
{
    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }
    
    public SupportTicketStatus? Status { get; set; }
    
    public long? AssignedAdminId { get; set; }
    
    public string? SubjectContains { get; set; }
    
    public string OrderBy { get; set; } = "id";
    
    public string OrderDir { get; set; } = "desc";
}