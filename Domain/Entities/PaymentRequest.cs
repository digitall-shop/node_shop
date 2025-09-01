using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;
using Domain.Enumes.Transaction;
using Domain.Events.DomainEvents.Events.Payment;

namespace Domain.Entities;
[Table("PaymentRequests" , Schema = "Payment")]
public class PaymentRequest :BaseEntity<long>  
{
    public decimal Amount { get; set; }
    public PaymentRequestStatus Status { get; set; } = PaymentRequestStatus.Pending;
    public PaymentMethod Method { get; set; }
    public string? ReceiptImageUrl { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? Description { get; set; }
    public long UserId { get; set; }
    public  User User { get; set; }
    public long? BankAccountId { get; set; }
    public  BankAccount? BankAccount { get; set; }
    
    
    
    public void SubmitReceipt(string? receiptUrl)
    {
        if (Status != PaymentRequestStatus.Pending)
            throw new InvalidOperationException("Only PENDING requests can submit receipt.");

        ReceiptImageUrl = receiptUrl;
        Status = PaymentRequestStatus.Submitted;
        ModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new PaymentRequestSubmittedEvent(this));
    }

    public void Approve()
    {
        if (Status != PaymentRequestStatus.Submitted)
            throw new InvalidOperationException("Only SUBMITTED requests can be approved.");

        Status = PaymentRequestStatus.Completed;
        ModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new PaymentApprovedEvent(this));
    }

    public void Reject(string reason)
    {
        if (Status != PaymentRequestStatus.Submitted)
            throw new InvalidOperationException("Only SUBMITTED requests can be rejected.");

        Description = reason;
        Status = PaymentRequestStatus.Failed;
        ModifiedDate = DateTime.UtcNow;

        AddDomainEvent(new PaymentRejectedEvent(this));
    }
}