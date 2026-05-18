using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Data;

public class AppDbSeeder
{
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
        await SeedGroupsAsync();
        await SeedUsersAsync();
        await SeedJunctionsAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[] { "Admin", "Viewer", "Editor" };

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
        if (await _db.Permissions.AnyAsync())
            return;

        _db.Permissions.AddRange(
            new PermissionsEntity
            {
                Id = Guid.NewGuid(),
                Name = "View Users",
                Resource = "users",
                Action = "read",
                Description = "Allows reading user records"
            },
            new PermissionsEntity
            {
                Id = Guid.NewGuid(),
                Name = "Manage Users",
                Resource = "users",
                Action = "write",
                Description = "Allows creating and updating users"
            },
            new PermissionsEntity
            {
                Id = Guid.NewGuid(),
                Name = "View Reports",
                Resource = "reports",
                Action = "read",
                Description = "Allows reading reports"
            }
        );

        await _db.SaveChangesAsync();
    }

    private async Task SeedPoliciesAsync()
    {
        if (await _db.Policies.AnyAsync())
            return;

        _db.Policies.AddRange(
            new PoliciesEntity { Id = Guid.NewGuid(), Name = "Read-only policy", Effect = "Allow" },
            new PoliciesEntity { Id = Guid.NewGuid(), Name = "Full-access policy", Effect = "Allow" }
        );

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
        await _userManager.AddToRoleAsync(admin, "Admin");
    }

    private async Task SeedJunctionsAsync()
    {
        var adminRole   = await _roleManager.FindByNameAsync("Admin");
        var viewerRole  = await _roleManager.FindByNameAsync("Viewer");
        var managePerm  = await _db.Permissions.FirstOrDefaultAsync(p => p.Action == "write");
        var readPolicy  = await _db.Policies.FirstOrDefaultAsync(p => p.Name == "Read-only policy");

        if (adminRole is null || managePerm is null || readPolicy is null)
            return;

        if (!await _db.RolePermissions.AnyAsync(rp => rp.RoleId == adminRole.Id))
        {
            _db.RolePermissions.Add(new RolePermissionEntity
            {
                RoleId = adminRole.Id,
                PermissionId = managePerm.Id
            });
        }

        if (!await _db.RolePolicies.AnyAsync(rp => rp.RoleId == adminRole.Id))
        {
            _db.RolePolicies.Add(new RolePolicyEntity
            {
                RoleId = adminRole.Id,
                PolicyId = readPolicy.Id
            });
        }

        await _db.SaveChangesAsync();
    }
}