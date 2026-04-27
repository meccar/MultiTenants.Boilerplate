using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Persistance.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistance.Repositories;

public class PermissionsRepository
    : RepositoryBase<PermissionsEntity, Guid>
        , IPermissionsRepository
{
    public PermissionsRepository(AppDbContext context)
        : base(context)
    {
    }
    
    public async Task<List<RolesEntity>> GetRolesByUserGroupsAsync(
        Guid userId, CancellationToken cancellationToken)
    {
        return await _context.UserGroups
            .Where(ug => ug.UserId == userId)
            .Join(_context.GroupRoles,
                ug => ug.GroupId,
                gr => gr.GroupId,
                (ug, gr) => gr.RoleId)
            .Join(_context.Roles,
                roleId => roleId,
                role => role.Id,
                (_, role) => (RolesEntity)role)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<IdentityRoleClaim<Guid>>> GetRoleClaimsByRolesAsync(
        IEnumerable<string> roleNames, CancellationToken cancellationToken)
    {
        return await _context.RoleClaims
            .Join(_context.Roles,
                rc => rc.RoleId,
                r  => r.Id,
                (rc, r) => new { rc, r.Name })
            .Where(x => roleNames.Contains(x.Name!))
            .Select(x => x.rc)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PermissionsEntity>> GetPermissionsByRolesAsync(
        IEnumerable<string> roleNames, CancellationToken cancellationToken)
    {
        return await _context.RolePermissions
            .Join(_context.Roles,
                rp => rp.RoleId,
                r  => r.Id,
                (rp, r) => new { rp.PermissionId, r.Name })
            .Where(x => roleNames.Contains(x.Name!))
            .Join(_context.Permissions,
                x  => x.PermissionId,
                p  => p.Id,
                (_, p) => p)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<PoliciesEntity>> GetPoliciesByRolesAsync(
        IEnumerable<string> roleNames, CancellationToken cancellationToken)
    {
        return await _context.RolePolicies
            .Join(_context.Roles,
                rp => rp.RoleId,
                r => r.Id,
                (rp, r) => new { rp.PolicyId, r.Name })
            .Where(x => roleNames.Contains(x.Name!))
            .Join(_context.Policies,
                x => x.PolicyId,
                p => p.Id,
                (_, p) => p)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
}
