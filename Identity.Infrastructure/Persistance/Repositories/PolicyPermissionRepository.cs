using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistance.Data;

namespace Identity.Infrastructure.Persistance.Repositories;

public class PolicyPermissionRepository
    : RepositoryBase<PolicyPermissionEntity, Guid>, IPolicyPermissionRepository
{
    public PolicyPermissionRepository(AppDbContext context)
        : base(context)
    {
    }
}