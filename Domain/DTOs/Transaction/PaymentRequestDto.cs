using Domain.Enumes.Transaction;

namespace Domain.DTOs.Transaction;

public class PaymentRequestDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public decimal Amount { get; set; }
    
    public PaymentMethod Method { get; set; }
    public PaymentRequestStatus Status { get; set; }
    public DateTime? CreateDate { get; set; }=DateTime.UtcNow;
    public string? GatewayTransactionId { get; set; }
    public string? ReceiptImageUrl { get; set; }
    public long? BankAccountId { get; set; }
    
    public string? Description { get; set; }
}