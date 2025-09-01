using Domain.Enumes.Transaction;

namespace Domain.DTOs.Transaction;

public class PaymentInitiationDto
{
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; }
}