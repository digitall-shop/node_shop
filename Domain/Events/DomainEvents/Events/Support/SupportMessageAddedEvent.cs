using MediatR;

namespace Application.DomainEvents.Events.Support;

public record SupportMessageAddedEvent(long TicketId, long MessageId, bool IsFromAdmin) : INotification;