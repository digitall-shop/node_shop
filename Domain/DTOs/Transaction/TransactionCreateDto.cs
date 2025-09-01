using Domain.Enumes.Transaction;

namespace Domain.DTOs.Transaction;

public class TransactionCreateDto
{
    public long UserId { get; set; }
    public long? PaymentRequestId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public TransactionReason Reason { get; set; }
    public string? Description { get; set; }
    public long BalanceBefore { get; set; }
    public long BalanceAfter { get; set; }
}