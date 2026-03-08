using MediatR;

namespace MultiTenants.Boilerplate.Domain.Seedwork.DomainEvent;
public interface IDomainEvent : INotification
{
    Guid Id { get; init; }
}
