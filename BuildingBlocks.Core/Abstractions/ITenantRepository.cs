
using BuildingBlocks.Core.Entities;
using BuildingBlocks.Core.Seedwork.Interface;

namespace BuildingBlocks.Core.Abstractions
{
    public interface ITenantRepository : IRepositoryBase<Tenants, Guid>
    {
        Task<Tenants?> GetByDomainAsync(string domain, CancellationToken cancellationToken = default);
        Task<Tenants?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
