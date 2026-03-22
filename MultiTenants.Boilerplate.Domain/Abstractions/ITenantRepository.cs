using MultiTenants.Boilerplate.Domain.Entities;
using MultiTenants.Boilerplate.Domain.Seedwork.Interface;

namespace MultiTenants.Boilerplate.Domain.Abstractions
{
    public interface ITenantRepository : IRepositoryBase<Tenants>
    {
        Task<Tenants?> GetByDomainAsync(string domain, CancellationToken cancellationToken = default);
        Task<Tenants?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
