namespace BuildingBlocks.Core.Seedwork.DomainEvent;
public abstract record DomainEvent(Guid Id) : IDomainEvent
{
    public long OccurredOn { get; init; } = DateTime.Now.Ticks;
}
