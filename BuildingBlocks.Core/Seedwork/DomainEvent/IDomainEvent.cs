using MediatR;

namespace BuildingBlocks.Core.Seedwork.DomainEvent;
public interface IDomainEvent : INotification
{
    Guid Id { get; init; }
    long OccurredOn { get; init; }
}
