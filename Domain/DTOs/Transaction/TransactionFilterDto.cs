using Domain.Common;
using Domain.Enumes.Transaction;

namespace Domain.DTOs.Transaction;

public class TransactionFilterDto : PagedDto
{
    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public TransactionType? Type { get; set; }

    public TransactionReason? Reason { get; set; }
    
    public decimal? FromAmount { get; set; }
    
    public decimal? ToAmount { get; set; }
    
    public string OrderBy { get; set; } = "id";
    
    public string OrderDir { get; set; } = "desc";
}