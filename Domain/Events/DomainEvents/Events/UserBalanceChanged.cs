using Domain.Enumes.Transaction;
using MediatR;

namespace Domain.Events.DomainEvents.Events;

public record UserBalanceChangedEvent : INotification
{
    public long UserId { get; init; }
    public long NewBalance { get; init; }
    public TransactionType Type { get; init; }
}