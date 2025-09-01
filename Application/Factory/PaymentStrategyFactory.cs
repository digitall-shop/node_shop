using Application.Services.Interfaces;
using Application.Services.Interfaces.Strategy;
using Domain.Enumes.Transaction;

namespace Application.Factory;

public class PaymentStrategyFactory(IEnumerable<IPaymentStrategy> strategies)
{
    public IPaymentStrategy GetStrategy(PaymentMethod method)
    {
        var strategy = strategies.FirstOrDefault(s => s.Method == method);
        if (strategy == null)
            throw new NotSupportedException($"Payment strategy {method} is not supported");
        return strategy;
    }
}