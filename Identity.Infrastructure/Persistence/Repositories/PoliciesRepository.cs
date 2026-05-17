using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistence.Data;

namespace Identity.Infrastructure.Persistence.Repositories;

public class PoliciesRepository
    : RepositoryBase<PoliciesEntity, Guid>,
        IPoliciesRepository
{
    public PoliciesRepository(AppDbContext context) 
        : base(context)
    {
    }
}