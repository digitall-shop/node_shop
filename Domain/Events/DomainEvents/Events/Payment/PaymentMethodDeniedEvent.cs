using Domain.Enumes.Transaction;
using MediatR;

namespace Domain.Events.DomainEvents.Events.Payment;

public record PaymentMethodDeniedEvent(long UserId, PaymentMethod Method) : INotification;