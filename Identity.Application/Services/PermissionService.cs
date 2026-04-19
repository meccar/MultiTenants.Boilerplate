using Identity.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Services;

public class PermissionService 
    : IPermissionService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public PermissionService(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<(IList<string> Roles, IList<string> Permissions)>
        GetUserRolesAndPermissionsAsync(Guid userId, Guid? tenantId,
            CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
                   ?? throw new UnauthorizedAccessException("User not found");

        // Roles assigned to this user
        var roles = await _userManager.GetRolesAsync(user);

        // 1. Permissions from user claims directly (AspNetUserClaims)
        var userClaims = await _userManager.GetClaimsAsync(user);
        var userPermissions = userClaims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value);

        // 2. Permissions from each role's claims (AspNetRoleClaims)
        var rolePermissions = new List<string>();
        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is null) continue;

            var roleClaims = await _roleManager.GetClaimsAsync(role);
            rolePermissions.AddRange(
                roleClaims
                    .Where(c => c.Type == "permission")
                    .Select(c => c.Value));
        }

        var allPermissions = userPermissions
            .Union(rolePermissions, StringComparer.OrdinalIgnoreCase)
            .Distinct()
            .ToList();

        return (roles, allPermissions);
    }
}
