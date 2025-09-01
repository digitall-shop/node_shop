namespace Domain.DTOs.Transaction;

public class TransactionDto
{
    public long Id { get; set; }
    
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty; 
    public string Reason { get; set; } = string.Empty;

    public string? Description { get; set; }
    
    public long BalanceBefore { get; set; }
    
    public long BalanceAfter { get; set; }
    
    public DateTime Timestamp { get; set; }
}