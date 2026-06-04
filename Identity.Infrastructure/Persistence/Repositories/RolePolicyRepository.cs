using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public class RolePolicyRepository
    : RepositoryBase<RolePolicyEntity, Guid>, IRolePolicyRepository
{
    public RolePolicyRepository(AppDbContext context) 
        : base(context)
    {
    }

    public async Task<List<PoliciesEntity>> GetPoliciesByRolesAsync(
        IList<string> roleNames,
        CancellationToken cancellationToken)
    {
        return await _context.RolePolicies
            .Join(_context.Roles,
                rolePolicy => rolePolicy.RoleId,
                role => role.Id,
                (rolePolicy, role) => new { rolePolicy.PolicyId, role.Name })
            .Where(x => x.Name != null && roleNames.Contains(x.Name))
            .Join(_context.Policies,
                x => x.PolicyId,
                policy => policy.Id,
                (_, policy) => policy)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}
