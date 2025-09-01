using Newtonsoft.Json;

namespace Domain.DTOs.Transaction.Plisio;

public class PlisioIpnDto
{
    [JsonProperty("txn_id")]
    public string? TransactionId { get; set; }

    [JsonProperty("ipn_type")]
    public string? IpnType { get; set; }

    [JsonProperty("merchant")]
    public string? Merchant { get; set; }

    [JsonProperty("merchant_id")]
    public string? MerchantId { get; set; }

    [JsonProperty("amount")]
    public decimal? Amount { get; set; }

    [JsonProperty("currency")]
    public string? Currency { get; set; }

    [JsonProperty("order_number")]
    public string? OrderNumber { get; set; }

    [JsonProperty("order_name")]
    public string? OrderName { get; set; }

    [JsonProperty("confirmations")]
    public int Confirmations { get; set; }

    [JsonProperty("status")]
    public string? Status { get; set; }

    [JsonProperty("source_currency")]
    public string? SourceCurrency { get; set; }

    [JsonProperty("source_amount")]
    public decimal? SourceAmount { get; set; }

    [JsonProperty("source_rate")]
    public decimal? SourceRate { get; set; }

    [JsonProperty("comment")]
    public string? Comment { get; set; }

    [JsonProperty("verify_hash")]
    public string? VerifyHash { get; set; }

    [JsonProperty("invoice_commission")]
    public decimal? InvoiceCommission { get; set; }

    [JsonProperty("invoice_sum")]
    public decimal? InvoiceSum { get; set; }

    [JsonProperty("invoice_total_sum")]
    public decimal? InvoiceTotalSum { get; set; }
}