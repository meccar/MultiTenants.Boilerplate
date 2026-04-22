using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace Identity.Application.Services;

public class PermissionService 
    : IPermissionService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IMemoryCache _cache;

    public PermissionService(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IMemoryCache cache
    ){
        _userManager = userManager;
        _roleManager = roleManager;
        _cache = cache;
    }

    public async Task<(IList<string> Roles, IList<string> Permissions)>
        GetUserRolesAndPermissionsAsync(
            Guid userId,
            Guid? tenantId,
            CancellationToken ct = default
    ){
        var cacheKey = $"user-permissions:{userId}:{tenantId}";

        if (_cache.TryGetValue(cacheKey,
                out (IList<string> Roles, IList<string> Permissions) cached))
            return cached;
        
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

        var result = (roles, (IList<string>)allPermissions);

        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
        
        return result;
    }
}
