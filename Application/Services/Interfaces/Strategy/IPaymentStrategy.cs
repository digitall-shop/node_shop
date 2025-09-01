using Domain.DTOs.Transaction;
using Domain.Enumes.Transaction;

namespace Application.Services.Interfaces.Strategy;

public interface IPaymentStrategy
{
    PaymentMethod Method { get; }

    /// <summary>
    /// this for start payment proses
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<PaymentResultDto> PaymentAsync(PaymentRequestDto request, CancellationToken ct = default);
}