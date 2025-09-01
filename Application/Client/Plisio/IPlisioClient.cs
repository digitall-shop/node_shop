using Domain.DTOs.Transaction.Plisio;

namespace Application.Client.Plisio;

public interface IPlisioClient
{
    /// <summary>
    /// this for creat a payment request and back url
    /// </summary>
    /// <param name="req"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<PlisioCreateInvoiceResponse> CreateInvoiceAsync(PlisioCreateInvoiceRequest req, CancellationToken ct = default);
}