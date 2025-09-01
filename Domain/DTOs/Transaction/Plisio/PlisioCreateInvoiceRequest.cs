namespace Domain.DTOs.Transaction.Plisio;

public class PlisioCreateInvoiceRequest
{
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }  
    public required string OrderName { get; init; }  
    public string? Description { get; init; }
    public required string CallbackUrl { get; init; } 
    public string? RedirectUrl { get; init; }         
    public string? SourceCurrency { get; init; }    
    public string? CurrencyTo { get; init; }         
    public Dictionary<string,string>? Extra { get; init; }  
}