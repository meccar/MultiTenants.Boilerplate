using MultiTenants.Boilerplate.Application.DTOs;
using MultiTenants.Boilerplate.Domain.Entities;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Abstractions;

/// <summary>
/// Application-facing identity operations that require tenant context or mapping to domain types.
/// Does not replace UserManager/SignInManager — only adds tenant-aware and domain-mapping operations.
/// Email confirmation, password reset, change password remain in Presentation with UserManager&lt;AppUser&gt;.
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Creates a user in the current tenant with the given email, userName, and optional password.
    /// Sets TenantId from current context before persisting.
    /// </summary>
    Task<Result<string>> CreateUserAsync(string email, string userName, string? password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by id (tenant-scoped by global query filter).
    /// </summary>
    Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by normalized email (tenant-scoped).
    /// </summary>
    Task<User?> GetUserByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by userName or email (tries userName first, then email). Used for login.
    /// Returns null if not found or not in current tenant.
    /// </summary>
    Task<User?> GetUserByUserNameOrEmailAsync(string userNameOrEmail, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates credentials for the given userNameOrEmail and password (tenant-scoped).
    /// </summary>
    Task<bool> ValidateCredentialsAsync(string userNameOrEmail, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a role to the user (tenant-scoped). Creates the role if it does not exist in the tenant.
    /// </summary>
    Task<Result> AssignRoleAsync(string userId, string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the list of role names for the user in the current tenant.
    /// </summary>
    Task<IReadOnlyList<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>Signs out the current user.</summary>
    Task SignOutAsync();

    /// <summary>
    /// Generates a password-reset token for the given email. Returns null if user not found.
    /// </summary>
    Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Resets a user's password using a reset token.</summary>
    Task<Result> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>Confirms a user's email address using a confirmation token.</summary>
    Task<Result> ConfirmEmailAsync(string userId, string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a new email confirmation token for the given email.
    /// Returns Failure if user not found or email already confirmed.
    /// </summary>
    Task<Result<string>> GenerateEmailConfirmationTokenAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Changes the password for the authenticated user and refreshes sign-in.</summary>
    Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>Returns account management info for the given user.</summary>
    Task<ManageInfoDto?> GetManageInfoAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>Updates email and/or username for the given user.</summary>
    Task<Result> UpdateManageInfoAsync(string userId, string? email, string? userName, CancellationToken cancellationToken = default);

    /// <summary>Validates password, deletes the user account, and signs out.</summary>
    Task<Result> DeletePersonalDataAsync(string userId, string password, CancellationToken cancellationToken = default);
}
