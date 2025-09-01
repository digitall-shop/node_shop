using Domain.Common;
using Domain.DTOs.Transaction;
using Domain.DTOs.Transaction.Plisio;
using Domain.Enumes.Transaction;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Interfaces;

public interface IPaymentService
{
    /// <summary>
    /// this for creat new payment request
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<PaymentResultDto> CreatePaymentAsync(PaymentInitiationDto request);
    /// <summary>
    /// this for approval a payment request 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> ApprovePaymentAsync(long id);

    /// <summary>
    /// this for reject a payment request
    /// </summary>
    /// <param name="rejection"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> RejectPaymentAsync(PaymentRejectionDto rejection,long id);
    
    /// <summary>
    /// this for submitting a payment request 
    /// </summary>
    /// <param name="paymentRequestId"></param>
    /// <param name="receiptFile"></param>
    /// <returns></returns>
    Task<bool> SubmitReceiptAsync(long paymentRequestId, IFormFile receiptFile);
    
    /// <summary>
    /// this for get a payment request by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<PaymentRequestDto> GetPaymentRequestByIdForUserAsync(long id);
    
    /// <summary>
    /// this for get a payment request by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<PaymentRequestDto> GetPaymentRequestByIdForAdminAsync(long id);
    
    /// <summary>
    /// this for get all payment requests for user  
    /// </summary>
    /// <returns></returns>
    Task<List<PaymentRequestDto>> GetUserPaymentRequestsAsync();

    /// <summary>
    /// this for get all payment requests for admin
    /// </summary>
    /// <returns></returns>
    Task<List<PaymentRequestDto>> GetPaymentRequestsForAdminAsync();
    
    /// <summary>
    /// this for 
    /// </summary>
    /// <param name="callbackData"></param>
    /// <returns></returns>
    Task ProcessPilisioCallbackAsync(PlisioIpnDto callbackData);
    
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
    Task<BasePaging<PaymentRequestDto>> GetPaymentsForAdminAsync(PaymentRequestFilterDto filter);
    
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
    Task<BasePaging<PaymentRequestDto>> GetPaymentsForUserAsync(PaymentRequestFilterDto filter);
    
    Task HandlePlisioIpnAsync(long paymentRequestId, PlisioIpnDto ipn);
}