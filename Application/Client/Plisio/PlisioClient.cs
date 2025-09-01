// Application/Client/Plisio/PlisioClient.cs
using System.Net.Http.Json;
using Application.Options;
using Domain.DTOs.Transaction.Plisio;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Client.Plisio;
public sealed class PlisioClient(HttpClient http, IOptions<PlisioOptions> opt, ILogger<PlisioClient> log)
    : IPlisioClient
{
    private readonly PlisioOptions _opt = opt.Value;

    public async Task<PlisioCreateInvoiceResponse> CreateInvoiceAsync(PlisioCreateInvoiceRequest req, CancellationToken ct = default)
    {
        var url = $"{_opt.ApiBase.TrimEnd('/')}/v1/invoices/new";

        var payload = new Dictionary<string, string?>
        {
            ["amount"] = req.Amount.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["order_name"] = req.OrderName,
            ["currency"] = req.Currency,
            ["callback_url"] = req.CallbackUrl,
            ["source_currency"] = req.SourceCurrency,
            ["currency_to"] = req.CurrencyTo,
            ["description"] = req.Description,
            ["redirect_to"] = req.RedirectUrl
        };

        if (req.Extra is not null)
            foreach (var kv in req.Extra)
                payload[kv.Key] = kv.Value;

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = new FormUrlEncodedContent(payload.Where(p => !string.IsNullOrWhiteSpace(p.Value))!);
        request.Headers.Add("X-Api-Key", _opt.ApiKey);

        var resp = await http.SendAsync(request, ct);
        if (!resp.IsSuccessStatusCode)
        {
            var errTxt = await resp.Content.ReadAsStringAsync(ct);
            log.LogError("Plisio invoice create failed: {Status} - {Body}", resp.StatusCode, errTxt);
            return new PlisioCreateInvoiceResponse { Status = false, Error = errTxt };
        }

        var json = await resp.Content.ReadFromJsonAsync<Dictionary<string, object?>>(cancellationToken: ct);

        var data = json?["data"] as Dictionary<string, object?>;
        var invoiceUrl = data?["invoice_url"]?.ToString();
        var txnId = data?["txn_id"]?.ToString();

        return new PlisioCreateInvoiceResponse
        {
            Status = true,
            InvoiceUrl = invoiceUrl,
            TxnId = txnId
        };
    }
}
