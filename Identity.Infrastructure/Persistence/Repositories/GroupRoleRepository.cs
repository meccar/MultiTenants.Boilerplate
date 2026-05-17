using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistence.Data;

namespace Identity.Infrastructure.Persistence.Repositories;

public class GroupRoleRepository
    : RepositoryBase<GroupRoleEntity, Guid>
        , IGroupRoleRepository
{
    public GroupRoleRepository(AppDbContext context) 
        : base(context)
    {
    }
}