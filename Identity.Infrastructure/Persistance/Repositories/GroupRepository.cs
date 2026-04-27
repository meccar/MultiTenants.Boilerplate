using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistance.Data;

namespace Identity.Infrastructure.Persistance.Repositories;

public class GroupRepository
    : RepositoryBase<GroupsEntity, Guid>, 
        IGroupsRepository
{
    public GroupRepository(AppDbContext context) 
        : base(context)
    {
    }
}