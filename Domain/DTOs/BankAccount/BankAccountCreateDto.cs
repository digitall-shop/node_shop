namespace Domain.DTOs.BankAccount;

public class BankAccountCreateDto
{
    public string? BankName { get; set; }
    
    public string? HolderName { get; set; }
    
    public string? CardNumber { get; set; }

    public bool IsActive { get; set; } = true;
}