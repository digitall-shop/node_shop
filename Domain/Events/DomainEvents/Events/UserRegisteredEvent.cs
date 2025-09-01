using MediatR;

namespace Domain.Events.DomainEvents.Events;

public sealed record UserRegisteredEvent(long UserId) : INotification;