using Domain.Enumes.Transaction;
using Domain.Enumes.User;

namespace Application.Extensiones
{
    public static class PaymentAccessExtensions
    {
        public static bool IsAllowed(this PaymentMethodAccess access, PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.CardToCard => access.HasFlag(PaymentMethodAccess.CardToCard),
                PaymentMethod.Plisio     => access.HasFlag(PaymentMethodAccess.Plisio),
                _ => false
            };
        }
    }
}