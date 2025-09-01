using MediatR;

namespace Domain.Events;


public abstract class EntityWithEvents : IDomainEvents
{
    private readonly List<INotification> _domainEvents = new();
    public List<INotification> DomainEvents => _domainEvents;

    public void AddDomainEvent(INotification @event) => _domainEvents.Add(@event);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
