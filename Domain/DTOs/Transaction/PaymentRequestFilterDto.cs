using Domain.Common;
using Domain.Enumes.Transaction;

namespace Domain.DTOs.Transaction;

public class PaymentRequestFilterDto : PagedDto
{
    
    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }
    
    public PaymentMethod? Method { get; set; }
    
    public PaymentRequestStatus? Status { get; set; }
    
    public decimal? FromAmount { get; set; }
    
    public decimal? ToAmount { get; set; }
    
    public string OrderBy { get; set; } = "id";
    
    public string OrderDir { get; set; } = "desc";
}