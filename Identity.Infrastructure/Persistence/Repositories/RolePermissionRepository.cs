using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public class RolePermissionRepository
    : RepositoryBase<RolePermissionEntity, Guid>, IRolePermissionRepository
{
    public RolePermissionRepository(AppDbContext context) 
        : base(context)
    {
    }
    
    public async Task<List<PoliciesEntity>> GetPoliciesByRolesAsync(
        IList<string> roleNames, CancellationToken cancellationToken)
    {
        return await _context.RolePermissions
            .Join(_context.Roles,
                rp => rp.RoleId,
                r => r.Id,
                (rp, r) => new { rp.PermissionId, r.Name })
            .Where(x => roleNames.Contains(x.Name!))
            .Join(_context.Policies,
                x => x.PermissionId,
                p => p.Id,
                (_, p) => p)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}