using MediatR;

namespace Domain.Events.DomainEvents.Events;

public record InstanceProvisionedEvent(
    long InstanceId,
    long PanelId,
    long NodeId) : INotification;