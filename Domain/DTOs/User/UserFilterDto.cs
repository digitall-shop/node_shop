using Domain.Common;

namespace Domain.DTOs.User;

public class UserFilterDto : PagedDto
{
    public long? UserId { get; set; }
    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }
    
    public decimal? FromBalance { get; set; }
    
    public decimal? ToBalance { get; set; }
    
    public string? UserName { get; set; }
    
    public string OrderBy { get; set; } = "id";
    
    public string OrderDir { get; set; } = "desc";
}