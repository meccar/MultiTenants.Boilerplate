using Identity.Domain.Interfaces;
using Identity.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Repositories
{
    public class UserAccountRepository
        : UserManager<IdentityUser>, IUserAccountRepository<IdentityUser>
    {
        public UserAccountRepository(
            IUserStore<IdentityUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<IdentityUser> passwordHasher,
            IEnumerable<IUserValidator<IdentityUser>> userValidators,
            IEnumerable<IPasswordValidator<IdentityUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<IdentityUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators,
                   passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        async Task<UserResult> IUserAccountRepository<IdentityUser>.CreateAsync(IdentityUser user, string password)
            => ToUserResult(await base.CreateAsync(user, password));

        async Task<UserResult> IUserAccountRepository<IdentityUser>.DeleteAsync(IdentityUser user)
            => ToUserResult(await base.DeleteAsync(user));

        async Task<UserResult> IUserAccountRepository<IdentityUser>.UpdateAsync(IdentityUser user)
            => ToUserResult(await base.UpdateAsync(user));

        async Task<UserResult> IUserAccountRepository<IdentityUser>.ChangePasswordAsync(IdentityUser user, string currentPassword, string newPassword)
            => ToUserResult(await base.ChangePasswordAsync(user, currentPassword, newPassword));

        async Task<UserResult> IUserAccountRepository<IdentityUser>.ResetPasswordAsync(IdentityUser user, string token, string newPassword)
            => ToUserResult(await base.ResetPasswordAsync(user, token, newPassword));

        async Task<UserResult> IUserAccountRepository<IdentityUser>.AddToRoleAsync(IdentityUser user, string role)
            => ToUserResult(await base.AddToRoleAsync(user, role));

        async Task<UserResult> IUserAccountRepository<IdentityUser>.RemoveFromRoleAsync(IdentityUser user, string role)
            => ToUserResult(await base.RemoveFromRoleAsync(user, role));

        async Task<UserResult> IUserAccountRepository<IdentityUser>.ConfirmEmailAsync(IdentityUser user, string token)
            => ToUserResult(await base.ConfirmEmailAsync(user, token));

        async Task<UserResult> IUserAccountRepository<IdentityUser>.SetLockoutEndDateAsync(IdentityUser user, DateTimeOffset? lockoutEnd)
            => ToUserResult(await base.SetLockoutEndDateAsync(user, lockoutEnd));

        public Task<IdentityUser?> FindByPhoneAsync(string phone)
            => Task.FromResult(Users.FirstOrDefault(u => u.PhoneNumber == phone));

        private static UserResult ToUserResult(IdentityResult result) =>
            result.Succeeded
                ? UserResult.Success()
                : UserResult.Failure(result.Errors.Select(e => e.Description));
    }
}
