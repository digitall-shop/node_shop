namespace Domain.DTOs.User;

public class UserUpdateCreditDto
{
    public long UserId { get; set; }
    public decimal AmountToChange { get; set; }
    
}