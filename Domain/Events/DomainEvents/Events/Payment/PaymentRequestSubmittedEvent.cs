using Domain.Entities;
using MediatR;

namespace Domain.Events.DomainEvents.Events.Payment;

public sealed record PaymentRequestSubmittedEvent(PaymentRequest PaymentRequest) : INotification;