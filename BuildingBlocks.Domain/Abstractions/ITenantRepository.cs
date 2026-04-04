using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Seedwork.Interface;

namespace BuildingBlocks.Domain.Abstractions
{
    public interface ITenantRepository : IRepositoryBase<Tenants>
    {
        Task<Tenants?> GetByDomainAsync(string domain, CancellationToken cancellationToken = default);
        Task<Tenants?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
