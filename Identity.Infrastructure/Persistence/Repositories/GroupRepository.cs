using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistence.Data;

namespace Identity.Infrastructure.Persistence.Repositories;

public class GroupRepository
    : RepositoryBase<GroupsEntity, Guid>, 
        IGroupsRepository
{
    public GroupRepository(AppDbContext context) 
        : base(context)
    {
    }
}