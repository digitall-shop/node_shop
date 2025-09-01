using Application.DomainEvents.Events;
using Application.Services.Interfaces;
using Domain.Enumes.Transaction;
using Domain.Events.DomainEvents.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.DomainEvents.Handlers;

public class UserBalanceChangedHandler(
    IInstanceService service,
    ILogger<IInstanceService> logger)
    : INotificationHandler<UserBalanceChangedEvent>
{
    public async Task Handle(UserBalanceChangedEvent notification, CancellationToken cancellationToken)
    {
        const long balanceThreshold = 10000;

        switch (notification.Type)
        {
            case TransactionType.Credit:
                logger.LogInformation("Credit event received. Resuming stopped services for user {UserId}",
                    notification.UserId);
                await service.ResumeStopedServices(notification.UserId);
                break;

            case TransactionType.Debit:
                if (notification.NewBalance < balanceThreshold)
                {
                    logger.LogInformation(
                        "Debit event received with low balance. Suspending services for user {UserId}",
                        notification.UserId);
                    await service.CheckAndSuspendInstancesAsync(notification.UserId);
                }
                break;
        }
    }
}