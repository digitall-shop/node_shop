using Application.Services.Interfaces;
using Application.Statics; 
using AutoMapper;
using Domain.Contract;
using Domain.DTOs.BankAccount;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations;

public class BankAccountService(
    IAsyncRepository<BankAccount, long> repository,
    IMapper mapper,
    ILogger<IBankAccountService> logger) : IBankAccountService
{
    public async Task<BankAccountDto> GetBankAccountByIdAsync(long id)
    {
        var bankAccount = await repository.GetByIdAsync(id);
        return mapper.Map<BankAccountDto>(bankAccount);
    }

    public async Task<List<BankAccountDto>> GetBankAllAccountsAsync()
    {
        var bankAccounts = await repository.GetAllAsync();
        return mapper.Map<List<BankAccountDto>>(bankAccounts);
    }

    public async Task<BankAccountDto> CreateBankAccountAsync(BankAccountCreateDto create)
    {
        var newAccount = mapper.Map<BankAccount>(create);

        await repository.AddEntity(newAccount);
        await repository.SaveChangesAsync();

        logger.BankAccountCreated(newAccount.Id, newAccount.IsActive);

        return mapper.Map<BankAccountDto>(newAccount);
    }

    public async Task<BankAccountDto> UpdateBankAccountAsync(long id, BankAccountUpdateDto update)
    {
        var account = await repository.GetByIdAsync(id)
                     ?? throw new NotFoundException($"account not found by id:{id}");

        mapper.Map(update, account);
        await repository.UpdateEntity(account);
        await repository.SaveChangesAsync();

        logger.BankAccountUpdated(account.Id);

        return mapper.Map<BankAccountDto>(account);
    }

    public async Task DeleteBankAccountAsync(long id)
    {
        var account = await repository.GetByIdAsync(id)
                     ?? throw new NotFoundException($"account not found by id:{id}");

        account.IsDelete = true;
        account.IsActive  = false;

        await repository.UpdateEntity(account);
        await repository.SaveChangesAsync();

        logger.BankAccountSoftDeleted(account.Id);
    }

    public async Task UnactivatedShowAsync()
    {
        try
        {
            var affected = await repository
                .GetQuery()
                .Where(b => !b.IsDelete && b.IsActive)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(b => b.IsActive, _ => false));

            logger.BankAccountsVisibilityChanged(false);

            // logger.LogDebug("All bank accounts deactivated. Affected={Affected}", affected);
        }
        catch (NotSupportedException)
        {
            var list = await repository
                .GetQuery()
                .Where(b => !b.IsDelete && b.IsActive)
                .ToListAsync();

            foreach (var b in list)
                b.IsActive = false;

            await repository.SaveChangesAsync();

            logger.BankAccountsVisibilityChanged(false);
        }
    }

    public async Task ActivateShowAsync()
    {
        try
        {
            var affected = await repository
                .GetQuery()
                .Where(b => !b.IsDelete && !b.IsActive)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(b => b.IsActive, _ => true));

            logger.BankAccountsVisibilityChanged(true);

            // (اختیاری) برای دیباگ محلی
            // logger.LogDebug("All bank accounts activated. Affected={Affected}", affected);
        }
        catch (NotSupportedException)
        {
            var list = await repository
                .GetQuery()
                .Where(b => !b.IsDelete && !b.IsActive)
                .ToListAsync();

            foreach (var b in list)
                b.IsActive = true;

            await repository.SaveChangesAsync();

            logger.BankAccountsVisibilityChanged(true);
        }
    }
}
