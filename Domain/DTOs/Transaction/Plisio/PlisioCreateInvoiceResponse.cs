namespace Domain.DTOs.Transaction.Plisio;

public class PlisioCreateInvoiceResponse
{
    public bool Status { get; init; }            
    public string? InvoiceUrl { get; init; }         
    public string? TxnId { get; init; }             
    public string? Error { get; init; }
}