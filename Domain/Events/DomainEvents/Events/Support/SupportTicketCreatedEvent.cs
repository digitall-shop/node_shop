using MediatR;

namespace Application.DomainEvents.Events.Support;

public record SupportTicketCreatedEvent(long TicketId) : INotification;