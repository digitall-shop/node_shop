using Domain.Entities;
using MediatR;

namespace Domain.Events.DomainEvents.Events.Payment;

public record PaymentRejectedEvent(PaymentRequest PaymentRequest) : INotification;