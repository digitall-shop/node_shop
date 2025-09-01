namespace Domain.DTOs.BankAccount;

public class BankAccountDto
{
    public long Id { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public string? BankName { get; set; }
    
    public string? HolderName { get; set; }
    
    public string? CardNumber { get; set; }
    
    public bool IsActive { get; set; }
}