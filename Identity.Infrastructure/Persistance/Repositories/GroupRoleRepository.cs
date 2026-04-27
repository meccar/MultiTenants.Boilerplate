using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistance.Data;

namespace Identity.Infrastructure.Persistance.Repositories;

public class GroupRoleRepository
    : RepositoryBase<GroupRoleEntity, Guid>
        , IGroupRoleRepository
{
    public GroupRoleRepository(AppDbContext context) 
        : base(context)
    {
    }
}