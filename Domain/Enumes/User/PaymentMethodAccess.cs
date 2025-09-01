namespace Domain.Enumes.User;

[Flags]
public enum PaymentMethodAccess
{
  
    None       = 0,
    CardToCard = 1 << 0, 
    Plisio     = 1 << 1,
    All        = CardToCard | Plisio
}
