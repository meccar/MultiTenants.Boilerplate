using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistance.Data;

namespace Identity.Infrastructure.Persistance.Repositories;

public class PoliciesRepository
    : RepositoryBase<PoliciesEntity, Guid>,
        IPoliciesRepository
{
    public PoliciesRepository(AppDbContext context) 
        : base(context)
    {
    }
}