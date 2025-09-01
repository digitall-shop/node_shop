using MediatR;

namespace Domain.Events;

public interface IDomainEvents
{
    List<INotification> DomainEvents { get; }
    void AddDomainEvent(INotification @event);
    void ClearDomainEvents();
   
   
}