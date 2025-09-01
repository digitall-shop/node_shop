using Application.Client.Plisio;
using Application.Options;
using Application.Services.Interfaces.Strategy;
using Application.Statics;
using Domain.Contract;
using Domain.DTOs.Transaction;
using Domain.DTOs.Transaction.Plisio;
using Domain.Enumes.Transaction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services.Implementations.Strategy;

public sealed class PlisioPaymentStrategy(
    IPlisioClient client,
    IUserContextService ctx,
    IOptions<PlisioOptions> opt,
    ILogger<PlisioPaymentStrategy> logger)
    : IPaymentStrategy
{
    private readonly PlisioOptions _opt = opt.Value;

    public PaymentMethod Method => PaymentMethod.Plisio;


    public async Task<PaymentResultDto> PaymentAsync(PaymentRequestDto reqDto, CancellationToken ct = default)
    {
        var callbackSecret = Guid.NewGuid().ToString("N");

        var callbackUrl = $"{_opt.CallbackBaseUrl}?prId={reqDto.Id}&secret={callbackSecret}";

        var createReq = new PlisioCreateInvoiceRequest
        {
            Amount = reqDto.Amount,
            Currency = "USD",                        
            OrderName = $"TopUp #{reqDto.Id}",
            Description = $"Account recharge by user {ctx.UserId}",
            CallbackUrl = callbackUrl,
            RedirectUrl = null,                      
            Extra = new Dictionary<string, string>    
            {
                ["pr_id"] = reqDto.Id.ToString(),
                ["user_id"] = ctx.UserId.ToString()
            }
        };

        var res = await client.CreateInvoiceAsync(createReq, ct);

        if (!res.Status || string.IsNullOrWhiteSpace(res.InvoiceUrl))
        {
            return new PaymentResultDto
            {
                IsSuccess = false,
                Description = res.Error ?? "Plisio: invoice create failed"
            };
        }

        reqDto.GatewayTransactionId = res.TxnId;
      

        logger.PaymentRequestCreated(reqDto.Id, reqDto.UserId, reqDto.Amount, Method.ToString());

        return new PaymentResultDto
        {
            IsSuccess = true,
            RedirectUrl = res.InvoiceUrl,
            Description = "OK"
        };
    }
}
