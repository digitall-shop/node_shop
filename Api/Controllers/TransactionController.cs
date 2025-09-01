using Api.Filters;
using Api.Securities;
using Application.Services.Interfaces;
using Domain.Common;
using Domain.DTOs.Transaction;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Tags("Transaction Endpoint")]
[Route("api/transaction")]
public class TransactionController(ITransactionService service) : ApiBaseController
{
    [HttpGet("{id:long}")]
    [EndpointName("transaction")]
    [EndpointSummary("get a transaction")]
    [EndpointDescription("admin and user can get a transaction")]
    [ProducesResponseType(typeof(ApiResult<TransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResult<TransactionDto>>> GetTransaction([FromRoute] long id)
    {
        var transaction = await service.GetTransactionByIdAsync(id);
        return Ok(transaction);
    }

    [HttpGet]
    [EndpointName("all-transactions")]
    [EndpointSummary("get all transactions")]
    [EndpointDescription("admin can get all transactions")]
    [SuperAdminAuthorize]
    [ProducesResponseType(typeof(ApiResult<BasePaging<TransactionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResult<BasePaging<TransactionDto>>>> GetAllTransactions([FromQuery] TransactionFilterDto filter)
    {
        var transactions = await service.GetTransactionsForAdminAsync(filter);
        return Ok(transactions);
    }

    [HttpGet("user")]
    [EndpointName("user-transactions")]
    [EndpointSummary("get all user transactions")]
    [EndpointDescription("user can get all transactions")]
    [ProducesResponseType(typeof(ApiResult<BasePaging<TransactionDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResult<BasePaging<TransactionDto>>>> GetAllUserTransactions(
        [FromQuery] TransactionFilterDto filter)
    {
        var transactions = await service.GetTransactionsForUserAsync(filter);
        return Ok(transactions);
    }

    [HttpPost("balance/credit")]
    [EndpointName("manual-credit")]
    [EndpointSummary("manual credit transaction")]
    [EndpointDescription("admin can crediting manual transaction")]
    [SuperAdminAuthorize]
    [ProducesResponseType(typeof(ApiResult<TransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResult<TransactionDto>>> ManualCreditTransaction(
        [FromBody] AdminManualAdjustDto credit)
    {
        var transaction = await service.CreateManualCreditAsync(credit);
        return Ok(transaction);
    }

    [HttpPost("balance/debit")]
    [EndpointName("manual-debit")]
    [EndpointSummary("manual debit transaction")]
    [EndpointDescription("admin can debiting manual transaction")]
    [SuperAdminAuthorize]
    [ProducesResponseType(typeof(ApiResult<TransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResult<TransactionDto>>> ManualDebitTransaction(
        [FromBody] AdminManualAdjustDto debit)
    {
        var transaction = await service.CreateManualDebitAsync(debit);
        return Ok(transaction);
    }
}