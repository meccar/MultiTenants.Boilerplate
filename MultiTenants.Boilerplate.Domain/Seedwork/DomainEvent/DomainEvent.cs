namespace MultiTenants.Boilerplate.Domain.Seedwork.DomainEvent;
public abstract record DomainEvent(Guid Id) : IDomainEvent;
