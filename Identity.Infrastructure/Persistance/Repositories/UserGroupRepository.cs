using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistance.Data;

namespace Identity.Infrastructure.Persistance.Repositories;

public class UserGroupRepository
    : RepositoryBase<UserGroupEntity, Guid>, IUserGroupRepository
{
    public UserGroupRepository(AppDbContext context) 
        : base(context)
    {
    }
}