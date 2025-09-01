using Domain.Enumes.User;

namespace Domain.DTOs.User;

public class UserCreateDto
{
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public required long ChatId { get; set; }
    
    public PaymentMethodAccess PaymentAccess { get; set; } = PaymentMethodAccess.Plisio;
}