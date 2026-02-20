using Microsoft.AspNetCore.Identity;
using MultiTenants.Boilerplate.Application.Abstractions;
using MultiTenants.Boilerplate.Domain.Entities;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Infrastructure.Identity;

/// <summary>
/// Implements IIdentityService using UserManager and RoleManager directly.
/// Only adds TenantId on create and maps AppUser → domain User; does not reimplement Identity.
/// </summary>
public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ITenantProvider _tenantProvider;

    public IdentityService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<AppRole> roleManager,
        ITenantProvider tenantProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tenantProvider = tenantProvider;
    }

    /// <inheritdoc />
    public async Task<Result<string>> CreateUserAsync(string email, string userName, string? password, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantProvider.GetCurrentTenantId();
        if (string.IsNullOrEmpty(tenantId))
            return Result<string>.Failure("Tenant context not found");

        var user = new AppUser
        {
            UserName = userName,
            Email = email,
            TenantId = tenantId
        };

        IdentityResult result = string.IsNullOrEmpty(password)
            ? await _userManager.CreateAsync(user)
            : await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
            return Result<string>.Success(user.Id);

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return Result<string>.Failure(errors);
    }

    /// <inheritdoc />
    public async Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        return appUser == null ? null : MapToDomainUser(appUser);
    }

    /// <inheritdoc />
    public async Task<User?> GetUserByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByEmailAsync(normalizedEmail);
        return appUser == null ? null : MapToDomainUser(appUser);
    }

    /// <inheritdoc />
    public async Task<User?> GetUserByUserNameOrEmailAsync(string userNameOrEmail, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByNameAsync(userNameOrEmail)
            ?? await _userManager.FindByEmailAsync(userNameOrEmail);
        return appUser == null ? null : MapToDomainUser(appUser);
    }

    /// <inheritdoc />
    public async Task<bool> ValidateCredentialsAsync(string userNameOrEmail, string password, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByNameAsync(userNameOrEmail)
            ?? await _userManager.FindByEmailAsync(userNameOrEmail);
        if (appUser == null)
            return false;

        var result = await _signInManager.CheckPasswordSignInAsync(appUser, password, lockoutOnFailure: false);
        return result.Succeeded;
    }

    /// <inheritdoc />
    public async Task<Result> AssignRoleAsync(string userId, string roleName, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantProvider.GetCurrentTenantId();
        if (string.IsNullOrEmpty(tenantId))
            return Result.Failure("Tenant context not found");

        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null)
            return Result.Failure("User not found");

        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            var newRole = new AppRole { Name = roleName, TenantId = tenantId };
            var createResult = await _roleManager.CreateAsync(newRole);
            if (!createResult.Succeeded)
                return Result.Failure(string.Join(", ", createResult.Errors.Select(e => e.Description)));
        }

        if (await _userManager.IsInRoleAsync(appUser, roleName))
            return Result.Success();

        var addResult = await _userManager.AddToRoleAsync(appUser, roleName);
        return addResult.Succeeded
            ? Result.Success()
            : Result.Failure(string.Join(", ", addResult.Errors.Select(e => e.Description)));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null)
            return Array.Empty<string>();

        var roles = await _userManager.GetRolesAsync(appUser);
        return roles.ToList();
    }

    private static User MapToDomainUser(AppUser appUser) => new()
    {
        Id = appUser.Id,
        TenantId = appUser.TenantId,
        Email = appUser.Email ?? string.Empty,
        UserName = appUser.UserName ?? string.Empty,
        FullName = null,
        EmailConfirmed = appUser.EmailConfirmed,
        CreatedAt = null
    };
}
