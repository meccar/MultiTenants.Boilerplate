using BuildingBlocks.Domain.Seedwork.DomainEvent;

namespace BuildingBlocks.Domain.Seedwork.Entity;
public abstract class EntityBase
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public void AddDomainEvent(IDomainEvent e) => _domainEvents.Add(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
    public void RemoveDomainEvent(IDomainEvent e) => _domainEvents.Remove(e);
}
