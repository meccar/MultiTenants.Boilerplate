using MediatR;

namespace MultiTenants.Boilerplate.Domain.Seedwork.DomainEvent;
public interface IDomainEventHandler<TEvent>
    : INotificationHandler<TEvent>
    where TEvent : IDomainEvent
{
}
