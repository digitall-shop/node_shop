namespace Domain.DTOs.CurrencyReta;

public class CurrencyRateDto
{
    public long Id { get; set; }
    public required string CurrencyCode { get; set; } 
    public long RateToBase { get; set; }
    public bool IsManual { get; set; }
}