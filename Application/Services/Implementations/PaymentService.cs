using Application.Extensiones;
using Application.Extensions;
using Application.Factory;
using Application.Infrastructure;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Contract;
using Domain.DTOs.Transaction;
using Domain.DTOs.Transaction.Plisio;
using Domain.Entities;
using Domain.Enumes.Transaction;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Statics;
using Domain.Events.DomainEvents.Events.Payment;

namespace Application.Services.Implementations;

public class PaymentService(
    IAsyncRepository<PaymentRequest, long> repository,
    PaymentStrategyFactory factory,
    IUserContextService userContextService,
    IMapper mapper,
    ITransactionService transactionService,
    IFileService fileService,
    IUserService userService,
    IMediator mediator,
    ILogger<IPaymentService> logger) : IPaymentService
{
    public async Task<PaymentResultDto> CreatePaymentAsync(PaymentInitiationDto request)
    {
        var currentUser = await userService.GetUserByIdAsync(userContextService.UserId);
        if (!currentUser.PaymentAccess.IsAllowed(request.Method))
        {
            logger.PaymentMethodDeniedAttempt(currentUser.Id, request.Method.ToString());
            await mediator.Publish(new PaymentMethodDeniedEvent(currentUser.Id, request.Method));
            throw new BadRequestException("Selected payment method is not enabled for your account.");
        }

        var newRequest = new PaymentRequest
        {
            UserId = userContextService.UserId,
            Amount = request.Amount,
            Method = request.Method,
            Status = PaymentRequestStatus.Pending,
            CreateDate = DateTime.UtcNow,
          
        };

        await repository.AddEntity(newRequest);
        await repository.SaveChangesAsync();

        logger.PaymentRequestCreated(newRequest.Id, newRequest.UserId, newRequest.Amount, newRequest.Method.ToString());

        var paymentRequestDto = mapper.Map<PaymentRequestDto>(newRequest);
        var strategy = factory.GetStrategy(request.Method);

        var result = await strategy.PaymentAsync(paymentRequestDto);
        result.PaymentRequestId = newRequest.Id;

        if (result.IsSuccess)
        {
            newRequest.GatewayTransactionId = paymentRequestDto.GatewayTransactionId;
            newRequest.ModifiedDate = DateTime.UtcNow;
            newRequest.BankAccountId = paymentRequestDto.BankAccountId;
            result.Description = newRequest.Description;

            await repository.UpdateEntity(newRequest);
            await repository.SaveChangesAsync();
        }
        else
        {
            newRequest.Status = PaymentRequestStatus.Failed;
            newRequest.ModifiedDate = DateTime.UtcNow;
            await repository.UpdateEntity(newRequest);
            await repository.SaveChangesAsync();
        }

        return result;
    }

    public async Task<bool> ApprovePaymentAsync(long id)
    {
        var request = await repository.GetByIdAsync(id)
                      ?? throw new NotFoundException("Payment request not found");

        if (request.Status != PaymentRequestStatus.Submitted)
            throw new BadRequestException("Request status is not valid for approval.");

        var transaction = new TransactionCreateDto
        {
            UserId = request.UserId,
            PaymentRequestId = request.Id,
            Amount = request.Amount,
            Type = TransactionType.Credit,
            Reason = TransactionReason.TopUp,
            Description = $"Account recharged via {request.Method}"
        };

        try
        {
            await transactionService.CreateTransactionAsync(transaction);
            
            request.Approve();
            await repository.UpdateEntity(request);
            await repository.SaveChangesAsync();

            logger.PaymentApproved(request.Id, request.UserId, request.Amount);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating the transaction for PaymentRequest {Id}", id);
            return false;
        }
    }

    public async Task<bool> RejectPaymentAsync(PaymentRejectionDto rejection, long id)
    {
        var request = await repository.GetByIdAsync(id)
                      ?? throw new NotFoundException("Payment request not found");

        if (request.Status != PaymentRequestStatus.Submitted)
            throw new BadRequestException("This request cannot be rejected at its current status.");

        if (rejection.Description is null)
            throw new BadRequestException("must have a description for rejection.");
        request.Reject(rejection.Description);
        await repository.UpdateEntity(request);
        await repository.SaveChangesAsync();

        logger.PaymentRejected(request.Id, request.UserId, request.Amount, rejection.Description);
        return true;
    }

    public async Task<bool> SubmitReceiptAsync(long paymentRequestId, IFormFile receipt)
    {
        var request = await repository.GetByIdAsync(paymentRequestId);
        if (request is null || request.UserId != userContextService.UserId)
            throw new NotFoundException("Payment request not found");

        if (request.Status != PaymentRequestStatus.Pending)
            return false;

        var uploadResult = await fileService.SaveFileAsync(
            receipt,
            PathExtensions.TransactionReceiptOrigin,
            [ FileType.Image ]
        );

        if (!uploadResult.Succeeded)
            throw new ApplicationException(uploadResult.ErrorMessage);

        var originServerPath = fileService.GetServerPath(uploadResult.FilePath);
        var thumbRelativePath = Path.Combine(
            PathExtensions.TransactionReceiptThumb,
            Path.GetFileName(uploadResult.FilePath) ?? throw new InvalidOperationException()
        );
        var thumbServerPath = fileService.GetServerPath(thumbRelativePath);
        await fileService.CreateThumbnailAsync(originServerPath, thumbServerPath, 150, 150);

        request.SubmitReceipt(uploadResult.FilePath);

        await repository.UpdateEntity(request);
        await repository.SaveChangesAsync();

        logger.PaymentReceiptSubmitted(request.Id, request.UserId, request.Amount);
        return true;
    }


    public async Task<PaymentRequestDto> GetPaymentRequestByIdForUserAsync(long id)
    {
        var request = await repository.GetSingleAsync(p => p.Id == id && p.UserId == userContextService.UserId);
        return mapper.Map<PaymentRequestDto>(request);
    }

    public async Task<PaymentRequestDto> GetPaymentRequestByIdForAdminAsync(long id)
    {
        var request = await repository.GetSingleAsync(p => p.Id == id);
        return mapper.Map<PaymentRequestDto>(request);
    }

    public async Task<List<PaymentRequestDto>> GetUserPaymentRequestsAsync()
    {
        var requests = await repository.GetQuery()
            .Where(p => p.UserId == userContextService.UserId)
            .ToListAsync();
        return mapper.Map<List<PaymentRequestDto>>(requests);
    }

    public async Task<List<PaymentRequestDto>> GetPaymentRequestsForAdminAsync()
    {
        var requests = await repository.GetAllAsync();
        return mapper.Map<List<PaymentRequestDto>>(requests);
    }

    public async Task ProcessPilisioCallbackAsync(PlisioIpnDto callbackData)
    {
        var request = await repository.GetSingleAsync(p => p.GatewayTransactionId == callbackData.TransactionId)
                      ?? throw new NotFoundException("transaction not found.");

        if (request.Status != PaymentRequestStatus.Pending)
            throw new ApplicationException("Only submitted transactions can be confirmed.");

        if (callbackData.Status == PlisioResponseEnum.Success.ToString())
        {
            var transactionDto = new TransactionCreateDto
            {
                UserId = request.UserId,
                PaymentRequestId = request.Id,
                Amount = request.Amount,
                Type = TransactionType.Credit,
                Reason = TransactionReason.TopUp,
                Description = "Account recharged via Pilisio"
            };
            await transactionService.CreateTransactionAsync(transactionDto);

            request.Status = PaymentRequestStatus.Completed;
            request.ModifiedDate = DateTime.UtcNow;

            logger.PaymentApproved(request.Id, request.UserId, request.Amount);
        }
        else
        {
            request.Status = PaymentRequestStatus.Failed;
            request.ModifiedDate = DateTime.UtcNow;

            logger.PaymentRejected(request.Id, request.UserId, request.Amount,
                $"Pilisio status: {callbackData.Status}");
        }

        await repository.UpdateEntity(request);
        await repository.SaveChangesAsync();
    }

    public async Task<BasePaging<PaymentRequestDto>> GetPaymentsForAdminAsync(PaymentRequestFilterDto filter)
    {
        var (filters, _) = filter.ToRepositoryParams();
        var data = await repository.GetPagedDataAsync(
            predicate: null,
            filters: filters,
            page: (filter.Skip / filter.Take) + 1,
            take: filter.Take);

        var items = mapper.Map<List<PaymentRequestDto>>(data.Items);

        return new BasePaging<PaymentRequestDto>
        {
            Items = items,
            Page = data.Page,
            Take = data.Take,
            TotalCount = data.TotalCount,
        };
    }

    public async Task<BasePaging<PaymentRequestDto>> GetPaymentsForUserAsync(PaymentRequestFilterDto filter)
    {
        var (filters, _) = filter.ToRepositoryParams();
        var data = await repository.GetPagedDataAsync(
            predicate: u => u.UserId == userContextService.UserId,
            filters: filters,
            page: (filter.Skip / filter.Take) + 1,
            take: filter.Take);

        var items = mapper.Map<List<PaymentRequestDto>>(data.Items);

        return new BasePaging<PaymentRequestDto>
        {
            Items = items,
            Page = data.Page,
            Take = data.Take,
            TotalCount = data.TotalCount,
        };
    }

    public async Task HandlePlisioIpnAsync(long paymentRequestId, PlisioIpnDto ipn)
    {
       var pr = await repository.GetByIdAsync(paymentRequestId)
                 ?? throw new NotFoundException("Payment request not found");

        if (pr.Status is PaymentRequestStatus.Completed or PaymentRequestStatus.Failed)
            return;
        
        var status = (ipn.Status ?? "").ToLowerInvariant();
        switch (status)
        {
            case "success":
            case "completed":
                pr.Approve(); 
                await repository.UpdateEntity(pr);
                await repository.SaveChangesAsync();

                var tx = new TransactionCreateDto
                {
                    UserId = pr.UserId,
                    PaymentRequestId = pr.Id,
                    Amount = pr.Amount,
                    Type = TransactionType.Credit,
                    Reason = TransactionReason.TopUp,
                    Description = "Account recharged via Plisio"
                };
                await transactionService.CreateTransactionAsync(tx);

                logger.PaymentApproved(pr.Id, pr.UserId, pr.Amount);
                break;

            case "failed":
            case "expired":
            case "canceled":
                pr.Reject("Plisio reported failed/expired/canceled");
                await repository.UpdateEntity(pr);
                await repository.SaveChangesAsync();

                logger.PaymentRejected(pr.Id, pr.UserId, pr.Amount, "Plisio: failed/expired/canceled");
                break;
        }
    }
}