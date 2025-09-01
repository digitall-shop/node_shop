using Domain.Enumes.User;

namespace Domain.DTOs.User;

public class UserDto
{
    public string? UserName { get; set; }
    public string? FirstName { get; set; }

    public bool? LowBalanceNotified { get; set; } = false;
    public PaymentMethodAccess PaymentAccess { get; set; }
    public bool IsSuperAdmin { get; set; }
    public string? LastName { get; set; }
    public long Id { get; set; }
    public decimal PriceMultiplier { get; set; }
    public decimal Credit { get; set; }
    public long Balance { get; set; }
    public bool IsBlocked { get; set; }
}