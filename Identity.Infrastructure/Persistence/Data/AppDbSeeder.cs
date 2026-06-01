using BuildingBlocks.Core.Aggregates;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Identity.Infrastructure.Persistence.Data;

public class AppDbSeeder
{
    private const string SuperAdmin = nameof(WellKnownPermissions.SuperAdmin);

    private readonly AppDbContext _db;
    private readonly UserManager<UsersEntity> _userManager;
    private readonly RoleManager<RolesEntity> _roleManager;

    public AppDbSeeder(
        AppDbContext db,
        UserManager<UsersEntity> userManager,
        RoleManager<RolesEntity> roleManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedPermissionsAsync();
        await SeedPoliciesAsync();
        await SeedPolicyPermissionsAsync();
        await SeedGroupsAsync();
        await SeedUsersAsync();
        await SeedJunctionsAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[] { SuperAdmin };

        foreach (var name in roles)
        {
            if (await _roleManager.RoleExistsAsync(name))
                continue;

            await _roleManager.CreateAsync(new RolesEntity
            {
                Id = Guid.NewGuid(),
                Name = name,
                NormalizedName = name.ToUpperInvariant(),
                CreatedAt = DateTime.Now.Ticks
            });
        }
    }

    private async Task SeedPermissionsAsync()
    {
        var existingPermissions = await _db.Permissions
            .Select(permission => new
            {
                permission.Resource,
                permission.Action
            })
            .ToListAsync();

        var existingPermissionNames = existingPermissions
            .Select(permission => ToPermissionName(permission.Resource, permission.Action))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var permissions = WellKnownPermissions.SuperAdmin
            .Where(permission => !existingPermissionNames.Contains(permission))
            .Select(CreatePermissionEntity)
            .ToList();

        if (permissions.Count == 0)
            return;

        _db.Permissions.AddRange(permissions);

        await _db.SaveChangesAsync();
    }

    private async Task SeedPoliciesAsync()
    {
        var existingPolicyNames = await _db.Policies
            .Select(policy => policy.Name)
            .ToListAsync();

        var policies = WellKnownPermissions.SuperAdmin
            .Where(permission => !existingPolicyNames.Contains(permission))
            .Select(CreatePolicyEntity)
            .ToList();

        if (policies.Count == 0)
            return;

        _db.Policies.AddRange(policies);

        await _db.SaveChangesAsync();
    }

    private async Task SeedGroupsAsync()
    {
        if (await _db.Groups.AnyAsync())
            return;

        _db.Groups.Add(new GroupsEntity
        {
            Id = Guid.NewGuid(),
            Name = "Default Group",
            Description = "All new users"
        });

        await _db.SaveChangesAsync();
    }

    private async Task SeedUsersAsync()
    {
        const string adminEmail = "admin@example.com";

        if (await _userManager.FindByEmailAsync(adminEmail) is not null)
            return;

        var admin = new UsersEntity
        {
            Id = Guid.NewGuid(),
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            CreatedAt = DateTime.Now.Ticks
        };

        await _userManager.CreateAsync(admin, "Admin@12345!");
        await _userManager.AddToRoleAsync(admin, SuperAdmin);
    }

    private async Task SeedJunctionsAsync()
    {
        var superAdminRole = await _roleManager.FindByNameAsync(SuperAdmin);
        var permissions = await _db.Permissions.ToListAsync();
        var superAdminPermissions = permissions
            .Where(permission => WellKnownPermissions.SuperAdmin.Contains(ToPermissionName(permission)))
            .ToList();
        var superAdminPolicies = await _db.Policies
            .Where(policy => WellKnownPermissions.SuperAdmin.Contains(policy.Name))
            .ToListAsync();

        if (superAdminRole is null || superAdminPermissions.Count == 0 || superAdminPolicies.Count == 0)
            return;

        var existingPermissionIds = await _db.RolePermissions
            .Where(rolePermission => rolePermission.RoleId == superAdminRole.Id)
            .Select(rolePermission => rolePermission.PermissionId)
            .ToListAsync();

        var rolePermissions = superAdminPermissions
            .Where(permission => !existingPermissionIds.Contains(permission.Id))
            .Select(permission => new RolePermissionEntity
            {
                RoleId = superAdminRole.Id,
                PermissionId = permission.Id
            })
            .ToList();

        if (rolePermissions.Count > 0)
        {
            _db.RolePermissions.AddRange(rolePermissions);
        }

        var existingPolicyIds = await _db.RolePolicies
            .Where(rolePolicy => rolePolicy.RoleId == superAdminRole.Id)
            .Select(rolePolicy => rolePolicy.PolicyId)
            .ToListAsync();

        var rolePolicies = superAdminPolicies
            .Where(policy => !existingPolicyIds.Contains(policy.Id))
            .Select(policy => new RolePolicyEntity
            {
                RoleId = superAdminRole.Id,
                PolicyId = policy.Id
            })
            .ToList();

        if (rolePolicies.Count > 0)
        {
            _db.RolePolicies.AddRange(rolePolicies);
        }

        await _db.SaveChangesAsync();
    }

    private async Task SeedPolicyPermissionsAsync()
    {
        var seededPermissionNames = WellKnownPermissions.SuperAdmin.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var allPermissions = await _db.Permissions.ToListAsync();
        var permissions = allPermissions
            .Where(permission => seededPermissionNames.Contains(ToPermissionName(permission)))
            .ToList();
        var policies = await _db.Policies
            .Where(policy => seededPermissionNames.Contains(policy.Name))
            .ToListAsync();

        if (permissions.Count == 0 || policies.Count == 0)
            return;

        var policyByName = policies.ToDictionary(policy => policy.Name);
        var policyIds = policies.Select(policy => policy.Id).ToList();
        var permissionIds = permissions.Select(permission => permission.Id).ToList();

        var existingPolicyPermissionKeys = await _db.PolicyPermissions
            .Where(policyPermission =>
                policyIds.Contains(policyPermission.PolicyId) &&
                permissionIds.Contains(policyPermission.PermissionId))
            .Select(policyPermission => new
            {
                policyPermission.PolicyId,
                policyPermission.PermissionId
            })
            .ToListAsync();

        var existingKeys = existingPolicyPermissionKeys
            .Select(policyPermission => (policyPermission.PolicyId, policyPermission.PermissionId))
            .ToHashSet();

        var policyPermissions = permissions
            .Select(permission => new
            {
                Name = ToPermissionName(permission),
                Permission = permission
            })
            .Where(item => policyByName.ContainsKey(item.Name))
            .Select(item => new
            {
                Policy = policyByName[item.Name],
                item.Permission
            })
            .Where(item => !existingKeys.Contains((item.Policy.Id, item.Permission.Id)))
            .Select(item => new PolicyPermissionEntity
            {
                PolicyId = item.Policy.Id,
                PermissionId = item.Permission.Id,
                Effect = "Allow",
                Conditions = JsonDocument.Parse("{}")
            })
            .ToList();

        if (policyPermissions.Count == 0)
            return;

        _db.PolicyPermissions.AddRange(policyPermissions);

        await _db.SaveChangesAsync();
    }

    private static PermissionsEntity CreatePermissionEntity(string permission)
    {
        var parts = permission.Split(':');

        return new PermissionsEntity
        {
            Id = Guid.NewGuid(),
            Resource = parts[0],
            Action = parts.Length > 1 ? parts[1] : string.Empty
        };
    }

    private static PoliciesEntity CreatePolicyEntity(string permission)
    {
        return new PoliciesEntity
        {
            Id = Guid.NewGuid(),
            Name = permission
        };
    }

    private static string ToPermissionName(PermissionsEntity permission)
        => ToPermissionName(permission.Resource, permission.Action);

    private static string ToPermissionName(string resource, string action)
        => $"{resource}:{action}";
}
