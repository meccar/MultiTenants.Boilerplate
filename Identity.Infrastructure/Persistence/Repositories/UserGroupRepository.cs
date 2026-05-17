using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistence.Data;

namespace Identity.Infrastructure.Persistence.Repositories;

public class UserGroupRepository
    : RepositoryBase<UserGroupEntity, Guid>, IUserGroupRepository
{
    public UserGroupRepository(AppDbContext context) 
        : base(context)
    {
    }
}