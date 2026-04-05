namespace BuildingBlocks.Domain.Seedwork.DomainEvent;
public abstract record DomainEvent(Guid Id) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}
