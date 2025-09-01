using Domain.DTOs.BankAccount;

namespace Application.Services.Interfaces;

public interface IBankAccountService
{
    /// <summary>
    /// this for get a bank account Information by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<BankAccountDto> GetBankAccountByIdAsync(long id);
    /// <summary>
    /// this for get all bank accounts Information's
    /// </summary>
    /// <returns></returns>
    Task<List<BankAccountDto>> GetBankAllAccountsAsync();
    
    /// <summary>
    /// this for create a new bank account  
    /// </summary>
    /// <param name="create"></param>
    /// <returns></returns>
    Task<BankAccountDto> CreateBankAccountAsync(BankAccountCreateDto create);
    
    /// <summary>
    /// this for update a bank account
    /// </summary>
    /// <param name="id"></param>
    /// <param name="update"></param>
    /// <returns></returns>
    Task<BankAccountDto> UpdateBankAccountAsync(long id, BankAccountUpdateDto update);
    
    /// <summary>
    /// this for soft delete a bank account 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteBankAccountAsync(long id);
    
    /// <summary>
    /// this for unactivated all bank accounts  
    /// </summary>
    /// <returns></returns>
    Task UnactivatedShowAsync();
    
    /// <summary>
    /// this for active show bank accounts
    /// </summary>
    /// <returns></returns>
    Task ActivateShowAsync();
}