using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public class PolicyPermissionRepository
    : RepositoryBase<PolicyPermissionEntity, Guid>, IPolicyPermissionRepository
{
    public PolicyPermissionRepository(AppDbContext context)
        : base(context)
    {
    }
    
    public async Task<List<PermissionsEntity>> GetPermissionsByPoliciesAsync(
        IEnumerable<Guid> policyIds,
        CancellationToken cancellationToken = default)
    {
        var ids = policyIds.ToHashSet();

        return await _context.PolicyPermissions
            .Where(x => ids.Contains(x.PolicyId))
            .Select(x => x.Permission)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}