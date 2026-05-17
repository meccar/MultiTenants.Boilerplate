using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistence.Data;

namespace Identity.Infrastructure.Persistence.Repositories;

public class PolicyPermissionRepository
    : RepositoryBase<PolicyPermissionEntity, Guid>, IPolicyPermissionRepository
{
    public PolicyPermissionRepository(AppDbContext context)
        : base(context)
    {
    }
}