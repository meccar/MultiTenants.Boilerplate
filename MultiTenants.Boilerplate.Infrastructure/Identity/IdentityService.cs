using Microsoft.AspNetCore.Identity;
using MultiTenants.Boilerplate.Application.Abstractions;
using MultiTenants.Boilerplate.Application.DTOs;
using MultiTenants.Boilerplate.Domain.Entities;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Infrastructure.Identity;

/// <summary>
/// Implements IIdentityService using UserManager, SignInManager, and RoleManager directly.
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

    /// <inheritdoc />
    public async Task SignOutAsync()
        => await _signInManager.SignOutAsync();

    /// <inheritdoc />
    public async Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null)
            return null;

        return await _userManager.GeneratePasswordResetTokenAsync(appUser);
    }

    /// <inheritdoc />
    public async Task<Result> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null)
            return Result.Failure("Invalid request.");

        var result = await _userManager.ResetPasswordAsync(appUser, token, newPassword);
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));
    }

    /// <inheritdoc />
    public async Task<Result> ConfirmEmailAsync(string userId, string code, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null)
            return Result.Failure("User not found.");

        var result = await _userManager.ConfirmEmailAsync(appUser, code);
        return result.Succeeded
            ? Result.Success()
            : Result.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));
    }

    /// <inheritdoc />
    public async Task<Result<string>> GenerateEmailConfirmationTokenAsync(string email, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser == null)
            return Result<string>.Failure("User not found.");

        if (appUser.EmailConfirmed)
            return Result<string>.Failure("Email is already confirmed.");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
        // Return userId|token so the handler can build the callback URL
        return Result<string>.Success($"{appUser.Id}|{token}");
    }

    /// <inheritdoc />
    public async Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null)
            return Result.Failure("User not found.");

        var result = await _userManager.ChangePasswordAsync(appUser, currentPassword, newPassword);
        if (!result.Succeeded)
            return Result.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));

        await _signInManager.RefreshSignInAsync(appUser);
        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<ManageInfoDto?> GetManageInfoAsync(string userId, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null)
            return null;

        return new ManageInfoDto
        {
            Id = appUser.Id,
            UserName = appUser.UserName,
            Email = appUser.Email,
            EmailConfirmed = appUser.EmailConfirmed,
            PhoneNumber = appUser.PhoneNumber,
            TwoFactorEnabled = appUser.TwoFactorEnabled
        };
    }

    /// <inheritdoc />
    public async Task<Result> UpdateManageInfoAsync(string userId, string? email, string? userName, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null)
            return Result.Failure("User not found.");

        if (!string.IsNullOrWhiteSpace(email) && email != appUser.Email)
        {
            var emailResult = await _userManager.SetEmailAsync(appUser, email);
            if (!emailResult.Succeeded)
                return Result.Failure(string.Join("; ", emailResult.Errors.Select(e => e.Description)));
            appUser.EmailConfirmed = false;
        }

        if (!string.IsNullOrWhiteSpace(userName) && userName != appUser.UserName)
        {
            var userNameResult = await _userManager.SetUserNameAsync(appUser, userName);
            if (!userNameResult.Succeeded)
                return Result.Failure(string.Join("; ", userNameResult.Errors.Select(e => e.Description)));
        }

        var updateResult = await _userManager.UpdateAsync(appUser);
        if (!updateResult.Succeeded)
            return Result.Failure(string.Join("; ", updateResult.Errors.Select(e => e.Description)));

        await _signInManager.RefreshSignInAsync(appUser);
        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result> DeletePersonalDataAsync(string userId, string password, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser == null)
            return Result.Failure("User not found.");

        var isValidPassword = await _userManager.CheckPasswordAsync(appUser, password);
        if (!isValidPassword)
            return Result.Failure("Password is required to delete your account.");

        var result = await _userManager.DeleteAsync(appUser);
        if (!result.Succeeded)
            return Result.Failure(string.Join("; ", result.Errors.Select(e => e.Description)));

        await _signInManager.SignOutAsync();
        return Result.Success();
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
