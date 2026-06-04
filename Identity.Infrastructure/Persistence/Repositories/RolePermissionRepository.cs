using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistence.Data;

namespace Identity.Infrastructure.Persistence.Repositories;

public class RolePermissionRepository
    : RepositoryBase<RolePermissionEntity, Guid>, IRolePermissionRepository
{
    public RolePermissionRepository(AppDbContext context) 
        : base(context)
    {
    }
}
