using Application.Services.Interfaces.Strategy;
using Domain.Contract;
using Domain.DTOs.Transaction;
using Domain.Entities;
using Domain.Enumes.Transaction;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations.Strategy;

public class CardToCardStrategy(IAsyncRepository<BankAccount,long> repository,ILogger<IPaymentStrategy> logger) : IPaymentStrategy
{
    public PaymentMethod Method => PaymentMethod.CardToCard;
    public async Task<PaymentResultDto> PaymentAsync(PaymentRequestDto request,CancellationToken cancellationToken)
    {
        var activeCards = await repository.GetAsync(b => b.IsActive);
        
        if (activeCards == null) return new PaymentResultDto { IsSuccess = false, ErrorMessage = "no active cards found" };
        
        var random = new Random();
        var selectedCard = activeCards[random.Next(activeCards.Count)];
        
        request.BankAccountId = selectedCard.Id;
       
        return new PaymentResultDto
        {
            IsSuccess = true,
            RequiresRedirect = false,
            DisplayData = new 
            {
                selectedCard.BankName,
                selectedCard.CardNumber,
                selectedCard.HolderName
            }
        };
    }
}