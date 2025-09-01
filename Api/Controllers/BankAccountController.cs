using Api.Filters;
using Api.Securities;
using Application.Services.Interfaces;
using Domain.DTOs.BankAccount;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Tags("BankAccount Endpoint")]
[Route("api/bankAccount")]
[SuperAdminAuthorize]
public class BankAccountController(IBankAccountService service) : ApiBaseController
{
    [HttpGet]
    [EndpointName("bankAccounts")]
    [EndpointSummary("get all bank accounts")]
    [EndpointDescription("this for get all bank accounts for admin")]
    [ProducesResponseType(typeof(ApiResult<List<BankAccountDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ApiResult<List<BankAccountDto>>> GetAllBankAccounts()
    {
        var accounts = await service.GetBankAllAccountsAsync();
        return Ok(accounts);
    }

    [HttpGet("{id:long}")]
    [EndpointName("bankAccount")]
    [EndpointSummary("get a bank account")]
    [EndpointDescription("this for get single bank account by id")]
    [ProducesResponseType(typeof(ApiResult<BankAccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ApiResult<BankAccountDto>> GetBankAccount(long id)
    {
        var account = await service.GetBankAccountByIdAsync(id);
        return Ok(account);
    }

    [HttpPost]
    [EndpointName("create")]
    [EndpointSummary("create a new bank account")]
    [EndpointDescription("admin can create new bank account")]
    [ProducesResponseType(typeof(ApiResult<BankAccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<BankAccountDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ApiResult<BankAccountDto>> CreateBankAccount([FromBody] BankAccountCreateDto create)
    {
        var newAccount = await service.CreateBankAccountAsync(create);
        return Ok(newAccount);
    }

    [HttpPatch("{id:long}")]
    [EndpointName("update")]
    [EndpointSummary("update an bank account")]
    [EndpointDescription("admin can update an exist bank account")]
    [ProducesResponseType(typeof(ApiResult<BankAccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<BankAccountDto>), StatusCodes.Status304NotModified)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ApiResult<BankAccountDto>> UpdateBankAccount([FromRoute] long id,
        [FromBody] BankAccountUpdateDto update)
    {
        var account = await service.UpdateBankAccountAsync(id, update);
        return Ok(account);
    }

    [HttpDelete("{id:long}")]
    [EndpointName("delete-bank-account")]
    [EndpointSummary("delete a bank account")]
    [EndpointDescription("admin can delete an exist bank account")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ApiResult> DeleteBankAccount([FromRoute] long id)
    {
        await service.DeleteBankAccountAsync(id);
        return Ok();
    }
    
    [HttpPost("activate-all")]
    [EndpointName("activate-all-bank-accounts")]
    [EndpointSummary("Activate (show) all bank accounts")]
    [EndpointDescription("Sets IsActive = true for all non-deleted bank accounts.")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ActivateAll()
    {
        await service.ActivateShowAsync();
        return Ok();
    }
    
    [HttpPost("deactivate-all")]
    [EndpointName("deactivate-all-bank-accounts")]
    [EndpointSummary("Deactivate (hide) all bank accounts")]
    [EndpointDescription("Sets IsActive = false for all non-deleted bank accounts.")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeactivateAll()
    {
        await service.UnactivatedShowAsync();
        return Ok();
    }
}