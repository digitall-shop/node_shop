using Domain.Common;
using Domain.DTOs.Transaction;

namespace Application.Services.Interfaces;

public interface ITransactionService
{
    /// <summary>
    ///this for creat new transaction
    /// </summary>
    /// <param name="create"></param>
    /// <returns></returns>
    Task<TransactionDto> CreateTransactionAsync(TransactionCreateDto create);
    
    /// <summary>
    /// this for get all transactions 
    /// </summary>
    /// <returns></returns>
    Task<List<TransactionDto>> GetAllTransactionsAsync();
    
    /// <summary>
    /// this for get all user transactions 
    /// </summary>
    /// <returns></returns>
    Task<List<TransactionDto>> GetAllUserTransactionsAsync();
    
    /// <summary>
    /// this for get a transaction by id 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<TransactionDto> GetTransactionByIdAsync(long id);
    
    /// <summary>
    /// this for create manual transaction: type Credit
    /// </summary>
    /// <param name="create"></param>
    /// <returns></returns>
    Task<TransactionDto> CreateManualCreditAsync(AdminManualAdjustDto create);
    /// <summary>
    /// this for create manual transaction: type Debit
    /// </summary>
    /// <param name="create"></param>
    /// <returns></returns>
    Task<TransactionDto> CreateManualDebitAsync(AdminManualAdjustDto create);
    
    /// <summary>
    /// Retrieves a paginated list of transactions made by users under the current agent.
    /// The results are filtered and paged according to the criteria provided in <paramref name="filter"/>.
    /// </summary>
    /// <param name="filter">
    /// Filter and paging options for retrieving subset transactions, e.g. page number, page size, status, date range.
    /// </param>
    /// <returns>
    /// A <see cref="BasePaging{T}"/> containing the agent’s sub-user transactions along with paging information.
    /// </returns>
    Task<BasePaging<TransactionDto>> GetTransactionsForAdminAsync(TransactionFilterDto filter);
    
    /// <summary>
    /// Retrieves a paginated list of transactions made by users under the current agent.
    /// The results are filtered and paged according to the criteria provided in <paramref name="filter"/>.
    /// </summary>
    /// <param name="filter">
    /// Filter and paging options for retrieving subset transactions, e.g. page number, page size, status, date range.
    /// </param>
    /// <returns>
    /// A <see cref="BasePaging{T}"/> containing the agent’s sub-user transactions along with paging information.
    /// </returns>
    Task<BasePaging<TransactionDto>> GetTransactionsForUserAsync(TransactionFilterDto filter);
}