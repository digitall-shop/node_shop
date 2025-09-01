namespace Domain.DTOs.CurrencyReta;

public class CurrencyRateCreateDto
{
    public string CurrencyCode { get; set; } = null!;
    public decimal RateToBase { get; set; }
    public bool IsManual { get; set; }
}