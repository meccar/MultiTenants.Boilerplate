using MediatR;

namespace BuildingBlocks.Core.Seedwork.DomainEvent;
public interface IDomainEventHandler<TEvent>
    : INotificationHandler<TEvent>
    where TEvent : IDomainEvent
{
}
