using Identity.Domain.Models;

namespace Identity.Domain.Interfaces
{
    public interface IUserAccountRepository<TUser> where TUser : class
    {
        // Query
        Task<TUser?> FindByIdAsync(string userId);
        Task<TUser?> FindByEmailAsync(string email);
        Task<TUser?> FindByNameAsync(string userName);
        Task<TUser?> FindByPhoneAsync(string phone);
        IQueryable<TUser> Users { get; }

        // Create / Delete
        Task<UserResult> CreateAsync(TUser user, string password);
        Task<UserResult> DeleteAsync(TUser user);

        // Update
        Task<UserResult> UpdateAsync(TUser user);
        Task<UserResult> ChangePasswordAsync(TUser user, string currentPassword, string newPassword);
        Task<UserResult> ResetPasswordAsync(TUser user, string token, string newPassword);

        // Roles
        Task<UserResult> AddToRoleAsync(TUser user, string role);
        Task<UserResult> RemoveFromRoleAsync(TUser user, string role);
        Task<IList<string>> GetRolesAsync(TUser user);
        Task<bool> IsInRoleAsync(TUser user, string role);

        // Password / Token
        Task<bool> CheckPasswordAsync(TUser user, string password);
        Task<string> GeneratePasswordResetTokenAsync(TUser user);
        Task<string> GenerateEmailConfirmationTokenAsync(TUser user);
        Task<UserResult> ConfirmEmailAsync(TUser user, string token);

        // Lockout
        Task<bool> IsLockedOutAsync(TUser user);
        Task<UserResult> SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd);
    }
}