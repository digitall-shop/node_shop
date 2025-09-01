using Application.DomainEvents.Events;
using Application.Extensiones;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Contract;
using Domain.DTOs.Transaction;
using Domain.Entities;
using Domain.Enumes.Transaction;
using Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Statics;
using Domain.Events.DomainEvents.Events; // ← فقط همین

namespace Application.Services.Implementations;

public class TransactionService(
    IAsyncRepository<Transaction, long> repository,
    ILogger<ITransactionService> logger,
    IUserService userService,
    IUserContextService userContextService,
    IMapper mapper,
    IMediator mediator) : ITransactionService
{
    public async Task<TransactionDto> CreateTransactionAsync(TransactionCreateDto create)
    {
        var user = await userService.GetUserByIdAsync(create.UserId);
        var amountChange = create.Type == TransactionType.Credit ? create.Amount : -create.Amount;

        create.BalanceBefore = user.Balance;
        create.BalanceAfter = (long)(user.Balance + amountChange);

        if (create.Type == TransactionType.Debit && user.Balance < create.Amount)
            throw new BadRequestException("user balance cannot be less than debit amount");

        var transaction = mapper.Map<Transaction>(create);
        transaction.Timestamp = DateTime.UtcNow;

        await repository.AddEntity(transaction);
        await repository.SaveChangesAsync();

        await userService.UpdateUserBalanceAsync(create.UserId, (long)amountChange);

        var balanceChangedEvent = new UserBalanceChangedEvent
        {
            UserId = create.UserId,
            NewBalance = create.BalanceAfter,
            Type = create.Type
        };
        await mediator.Publish(balanceChangedEvent);

        logger.TransactionCreated(
            userId: create.UserId,
            amount: create.Amount,
            type: create.Type.ToString(),
            reason: create.Reason.ToString(),
            balanceBefore: create.BalanceBefore,
            balanceAfter: create.BalanceAfter
        );

        return mapper.Map<TransactionDto>(transaction);
    }

    public async Task<List<TransactionDto>> GetAllTransactionsAsync()
    {
        logger.LogInformation("get all transaction for admin");
        var transactions = await repository.GetAllAsync();
        return mapper.Map<List<TransactionDto>>(transactions);
    }

    public async Task<List<TransactionDto>> GetAllUserTransactionsAsync()
    {
        logger.LogInformation("get all user transactions");
        var transactions = await repository.GetQuery()
            .Where(t => t.UserId == userContextService.UserId)
            .ToListAsync();

        return mapper.Map<List<TransactionDto>>(transactions);
    }

    public async Task<TransactionDto> GetTransactionByIdAsync(long id)
    {
        logger.LogInformation("get a transaction by id:{id}", id);

        var transaction = await repository.GetByIdAsync(id)
                          ?? throw new NotFoundException("transaction not found");
        return mapper.Map<TransactionDto>(transaction);
    }

    public async Task<TransactionDto> CreateManualCreditAsync(AdminManualAdjustDto create)
    {
        if (create.Amount <= 0) throw new BadRequestException("Amount must be positive.");

        var transaction = mapper.Map<TransactionCreateDto>(create);
        transaction.Type = TransactionType.Credit;
        transaction.Reason = TransactionReason.ManualCredit;

        logger.LogInformation("Admin manual credit requested: Amount={Amount}, TargetUser={UserId}", create.Amount,
            create.UserId);
        return await CreateTransactionAsync(transaction);
    }

    public async Task<TransactionDto> CreateManualDebitAsync(AdminManualAdjustDto create)
    {
        if (create.Amount <= 0) throw new BadRequestException("Amount must be positive.");
        if (string.IsNullOrWhiteSpace(create.Description)) throw new BadRequestException("Description is required.");

        var transaction = mapper.Map<TransactionCreateDto>(create);
        transaction.Type = TransactionType.Debit;
        transaction.Reason = TransactionReason.ManualDebit;

        logger.LogInformation("Admin manual debit requested: Amount={Amount}, TargetUser={UserId}, Desc={Desc}",
            create.Amount, create.UserId, create.Description);

        return await CreateTransactionAsync(transaction);
    }

    public async Task<BasePaging<TransactionDto>> GetTransactionsForAdminAsync(TransactionFilterDto filter)
    {
        var (filters, _) = filter.ToRepositoryParams();
        var data = await repository.GetPagedDataAsync(
            predicate: null,
            filters: filters,
            page: (filter.Skip / filter.Take) + 1,
            take: filter.Take);

        var items = mapper.Map<List<TransactionDto>>(data.Items);

        return new BasePaging<TransactionDto>
        {
            Items = items,
            Page = data.Page,
            Take = data.Take,
            TotalCount = data.TotalCount,
        };
    }

    public async Task<BasePaging<TransactionDto>> GetTransactionsForUserAsync(TransactionFilterDto filter)
    {
        var (filters, _) = filter.ToRepositoryParams();

        var data = await repository.GetPagedDataAsync(
            predicate: u => u.UserId == userContextService.UserId,
            filters: filters,
            page: (filter.Skip / filter.Take) + 1,
            take: filter.Take);

        var items = mapper.Map<List<TransactionDto>>(data.Items);

        return new BasePaging<TransactionDto>
        {
            Items = items,
            Page = data.Page,
            Take = data.Take,
            TotalCount = data.TotalCount,
        };
    }
}