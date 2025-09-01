namespace Domain.DTOs.Transaction;

public class AdminManualAdjustDto
{
    public long UserId { get; set; }
    public decimal Amount { get; set; }
    public required string Description { get; set; }
}