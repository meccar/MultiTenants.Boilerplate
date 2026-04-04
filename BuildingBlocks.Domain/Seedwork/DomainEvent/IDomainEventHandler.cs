using MediatR;

namespace BuildingBlocks.Domain.Seedwork.DomainEvent;
public interface IDomainEventHandler<TEvent>
    : INotificationHandler<TEvent>
    where TEvent : IDomainEvent
{
}
