using MediatR;

namespace BuildingBlocks.Core.Seedwork.DomainEvent;
public interface IDomainEvent : INotification
{
    Guid Id { get; init; }
    DateTime OccurredOn { get; init; }
}
