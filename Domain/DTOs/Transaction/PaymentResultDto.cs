namespace Domain.DTOs.Transaction;

public class PaymentResultDto
{
    public long PaymentRequestId  { get; set; }
    public bool IsSuccess { get; set; }
    public bool RequiresRedirect { get; set; }
    public string? RedirectUrl { get; set; } 
    public object? DisplayData { get; set; }
    public string? Description { get; set; }
    public string? ErrorMessage { get; set; }
}