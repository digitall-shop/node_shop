using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;

namespace Domain.Entities;

[Table("CurrencyRates", Schema = "Setting")]
public class CurrencyRate : BaseEntity<long>
{
    public required string CurrencyCode { get; set; }
    public long RateToBase { get; set; }
    public bool IsManual { get; set; }
    
    public long CreateBy { get; set; }
    
    public long ModifyBy { get; set; }
    
    public bool IsDeleted { get; set; }
}