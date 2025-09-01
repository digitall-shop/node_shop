using Domain.Entities;
using MediatR;

namespace Domain.Events.DomainEvents.Events.Payment;

public record PaymentApprovedEvent(PaymentRequest PaymentRequest) : INotification;