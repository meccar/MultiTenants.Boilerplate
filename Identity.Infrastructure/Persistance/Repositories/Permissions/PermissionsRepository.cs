using Identity.Domain.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistance.Data;

namespace Identity.Infrastructure.Persistance.Repositories.Permissions;

public class PermissionsRepository
    : RepositoryBase<PermissionsEntity, int>
        , IPermissionsRepository
{
    public PermissionsRepository(AppDbContext context)
        : base(context)
    {
    }
}
