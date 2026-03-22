using Microsoft.EntityFrameworkCore;
using MultiTenants.Boilerplate.Domain.Abstractions;
using MultiTenants.Boilerplate.Domain.Entities;
using MultiTenants.Boilerplate.Domain.Seedwork.Interface;
using MultiTenants.Boilerplate.Infrastructure.Persistance.Data;

namespace MultiTenants.Boilerplate.Infrastructure.Persistance.Repositories;

public class TenantRepository : RepositoryBase<Tenants>, ITenantRepository
{
    public TenantRepository(
        IUnitOfWork unitOfWork, AppDbContext context)
        : base(context, unitOfWork)
    {
    }

    public async Task<Tenants?> GetByDomainAsync(
        string domain, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(t => t.Domain == domain, cancellationToken);

    public async Task<Tenants?> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
}
