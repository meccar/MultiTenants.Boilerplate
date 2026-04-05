using MediatR;

namespace BuildingBlocks.Domain.Seedwork.DomainEvent;
public interface IDomainEvent : INotification
{
    Guid Id { get; init; }
    DateTime OccurredOn { get; init; }
}
