using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Enumes.Transaction;

namespace Domain.Entities;
[Table("Transactions", Schema = "Payment")]
public class Transaction : BaseEntity<long>
{
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public TransactionReason Reason { get; set; }
    
    public long BalanceBefore { get; set; }
    
    public long BalanceAfter { get; set; }
    public string? Description { get; set; }
    public DateTime Timestamp { get; set; }
    public long UserId { get; set; }
    public virtual User User { get; set; }
    public long? PaymentRequestId { get; set; }
    public PaymentRequest? PaymentRequest { get; set; }
}