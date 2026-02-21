using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiTenants.Boilerplate.Infrastructure.Identity;

namespace MultiTenants.Boilerplate.Infrastructure.Data;

/// <summary>
/// Seeds default tenant (TAF), Admin role, and admin user using Finbuckle tenant store
/// and ASP.NET Core Identity (UserManager/RoleManager). Idempotent and safe to run multiple times.
/// </summary>
public sealed class DataSeeder
{
    private const string TafIdentifier = "taf";
    private const string TafName = "TAF";
    private const string AdminRoleName = "Admin";
    private const string AdminEmail = "admin@gmail.com";
    private const string AdminPassword = "Password@123";
    private const string NormalizedAdminEmail = "ADMIN@GMAIL.COM";
    private const string NormalizedAdminRoleName = "ADMIN";

    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly AppDbContext _dbContext;
    private readonly IMultiTenantStore<TenantInfo> _tenantStore;
    private readonly IMultiTenantContextSetter _tenantContextSetter;
    private readonly IMultiTenantContextAccessor<TenantInfo> _tenantContextAccessor;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        AppDbContext dbContext,
        IMultiTenantStore<TenantInfo> tenantStore,
        IMultiTenantContextSetter tenantContextSetter,
        IMultiTenantContextAccessor<TenantInfo> tenantContextAccessor,
        ILogger<DataSeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
        _tenantStore = tenantStore;
        _tenantContextSetter = tenantContextSetter;
        _tenantContextAccessor = tenantContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Seeds tenant TAF, Admin role, admin user, and role assignment in order.
    /// Idempotent: skips creation when tenant/role/user already exist.
    /// </summary>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // Use tenant from context if already set (e.g. by Program.cs before resolving DataSeeder).
        // This ensures DbContext was created with correct CurrentTenantId so role/user lookups work.
        var tenant = _tenantContextAccessor.MultiTenantContext?.TenantInfo;

        if (tenant == null)
        {
            // Step 1: Tenant (via Finbuckle tenant store) and set context
            try
            {
                var existing = await _tenantStore.GetByIdentifierAsync(TafIdentifier);
                if (existing != null)
                {
                    _logger.LogInformation("Tenant {Name} already exists (Id: {TenantId}). Skipping tenant creation.", TafName, existing.Id);
                    tenant = existing;
                }
                else
                {
                    var tenantId = Guid.NewGuid().ToString();
                    tenant = new TenantInfo
                    {
                        Id = tenantId,
                        Identifier = TafIdentifier,
                        Name = TafName
                    };
                    var added = await _tenantStore.AddAsync(tenant);
                    if (!added)
                        throw new InvalidOperationException("Tenant store failed to add tenant " + TafName);
                    _logger.LogInformation("Created tenant {Name} with Id: {TenantId}", TafName, tenantId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to ensure tenant {Name}.", TafName);
                throw;
            }

            _tenantContextSetter.MultiTenantContext = new MultiTenantContext<TenantInfo>(tenant);
        }
        else
        {
            _logger.LogInformation("Using tenant {Name} (Id: {TenantId}) from current context.", tenant.Name, tenant.Id);
        }

        if (tenant == null)
            throw new InvalidOperationException("Tenant context is required for seeding.");

        // Early exit: if admin user (NormalizedEmail = ADMIN@GMAIL.COM) and Admin role (NormalizedName = ADMIN) already exist in DB, skip seeding.
        // Uses IgnoreQueryFilters so we see rows from any tenant (in-memory tenant store resets on restart, so tenant id changes each run).
        var adminUserExists = await _dbContext.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.NormalizedEmail == NormalizedAdminEmail, cancellationToken);
        var adminRoleExists = await _dbContext.Roles
            .IgnoreQueryFilters()
            .AnyAsync(r => r.NormalizedName == NormalizedAdminRoleName, cancellationToken);
        if (adminUserExists && adminRoleExists)
        {
            _logger.LogInformation("Seed data already present: user NormalizedEmail={Email} and role NormalizedName={Role} exist. Skipping seed.", NormalizedAdminEmail, NormalizedAdminRoleName);
            return;
        }

        // Step 2: Admin role (AppRole with TenantId set before create)
        try
        {
            var existingRole = await _roleManager.FindByNameAsync(AdminRoleName);
            if (existingRole != null)
            {
                _logger.LogInformation("Role {RoleName} already exists for tenant {TenantId}. Skipping role creation.", AdminRoleName, tenant.Id);
            }
            else
            {
                var role = new AppRole
                {
                    Name = AdminRoleName,
                    NormalizedName = AdminRoleName.ToUpperInvariant(), // Required for UserManager.AddToRoleAsync lookup
                    TenantId = tenant.Id
                };
                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                    throw new InvalidOperationException("Role creation failed: " + string.Join("; ", result.Errors.Select(e => e.Description)));
                _logger.LogInformation("Created role {RoleName} for tenant {TenantId}.", AdminRoleName, tenant.Id);
            }
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Failed to ensure role {RoleName}.", AdminRoleName);
            throw;
        }

        // Step 3: Admin user (AppUser with TenantId set before create, password via UserManager)
        AppUser? user;
        try
        {
            user = await _userManager.FindByEmailAsync(AdminEmail);
            if (user != null)
            {
                _logger.LogInformation("User {Email} already exists for tenant {TenantId}. Skipping user creation.", AdminEmail, tenant.Id);
            }
            else
            {
                user = new AppUser
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    TenantId = tenant.Id
                };
                var result = await _userManager.CreateAsync(user, AdminPassword);
                if (!result.Succeeded)
                    throw new InvalidOperationException("User creation failed: " + string.Join("; ", result.Errors.Select(e => e.Description)));
                _logger.LogInformation("Created user {Email} for tenant {TenantId}.", AdminEmail, tenant.Id);
            }
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Failed to ensure user {Email}.", AdminEmail);
            throw;
        }

        if (user == null)
            return;

        // Step 4: Assign Admin role to admin user (role already created in Step 2 with NormalizedName set)
        try
        {
            var inRole = await _userManager.IsInRoleAsync(user, AdminRoleName);
            if (inRole)
            {
                _logger.LogInformation("User {Email} already has role {RoleName}. Skipping role assignment.", AdminEmail, AdminRoleName);
            }
            else
            {
                var result = await _userManager.AddToRoleAsync(user, AdminRoleName);
                if (!result.Succeeded)
                    throw new InvalidOperationException("Role assignment failed: " + string.Join("; ", result.Errors.Select(e => e.Description)));
                _logger.LogInformation("Assigned role {RoleName} to user {Email}.", AdminRoleName, AdminEmail);
            }
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Failed to assign role {RoleName} to user {Email}.", AdminRoleName, AdminEmail);
            throw;
        }

        // Step 5: Verify user can be found via UserManager scoped to tenant TAF
        try
        {
            var found = await _userManager.FindByEmailAsync(AdminEmail);
            if (found != null)
                _logger.LogInformation("Verification passed: user {Email} found via UserManager for tenant TAF.", AdminEmail);
            else
                _logger.LogWarning("Verification failed: user {Email} could not be found via UserManager after seeding.", AdminEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Verification step failed when looking up user {Email}.", AdminEmail);
            throw;
        }
    }
}
