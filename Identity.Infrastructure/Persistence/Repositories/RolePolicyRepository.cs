using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistence.Data;

namespace Identity.Infrastructure.Persistence.Repositories;

public class RolePolicyRepository
    : RepositoryBase<RolePolicyEntity, Guid>, IRolePolicyRepository
{
    public RolePolicyRepository(AppDbContext context) 
        : base(context)
    {
    }
}