using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Security.Claims;

namespace MultiTenants.Boilerplate.Application.Stores;

public class MongoUserStore : IUserPasswordStore<IdentityUser>, IUserEmailStore<IdentityUser>, IUserClaimStore<IdentityUser>
{
  private readonly ILogger<MongoUserStore> _logger;
    private readonly IMongoCollection<IdentityUser> _users;

    public MongoUserStore(
        IMongoDatabase database,
        ILogger<MongoUserStore> logger
    )
    {
      _logger = logger;
        _users = database.GetCollection<IdentityUser>("Users");
    }
    // Note: Finbuckle's WithPerTenantAuthentication automatically handles tenant isolation
    // The UserManager layer ensures all operations are tenant-scoped

    public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        try
        {
            await _users.InsertOneAsync(user, cancellationToken: cancellationToken);
            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }

    public async Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        try
        {
            await _users.DeleteOneAsync(u => u.Id == user.Id, cancellationToken);
            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user");
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }

    public async Task<IdentityUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await _users.Find(u => u.Id == userId).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IdentityUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return await _users.Find(u => u.NormalizedUserName == normalizedUserName).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.NormalizedUserName ?? user.UserName?.ToUpperInvariant());
    }

    public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id);
    }

    public Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName);
    }

    public Task SetNormalizedUserNameAsync(IdentityUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    public Task SetUserNameAsync(IdentityUser user, string? userName, CancellationToken cancellationToken)
    {
        user.UserName = userName ?? string.Empty;
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        try
        {
            await _users.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken: cancellationToken);
            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user");
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }

    public Task SetPasswordHashAsync(IdentityUser user, string? passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
        return Task.CompletedTask;
    }

    public Task<string?> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash);
    }

    public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
    }

    public async Task<IdentityUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        return await _users.Find(u => u.NormalizedEmail == normalizedEmail).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<string?> GetEmailAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Email);
    }

    public Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.EmailConfirmed);
    }

    public Task<string?> GetNormalizedEmailAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Email?.ToUpperInvariant());
    }

    public Task SetEmailAsync(IdentityUser user, string? email, CancellationToken cancellationToken)
    {
        user.Email = email ?? string.Empty;
        return Task.CompletedTask;
    }

    public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken)
    {
        user.EmailConfirmed = confirmed;
        return Task.CompletedTask;
    }

    public Task SetNormalizedEmailAsync(IdentityUser user, string? normalizedEmail, CancellationToken cancellationToken)
    {
        // Normalized email is computed, not stored
        return Task.CompletedTask;
    }

    public Task<IList<Claim>> GetClaimsAsync(IdentityUser user, CancellationToken cancellationToken)
    {
        // For simplicity, returning empty list. Can be extended to store claims in MongoDB
        return Task.FromResult<IList<Claim>>(new List<Claim>());
    }

    public Task AddClaimsAsync(IdentityUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        // Can be implemented to store claims in MongoDB
        return Task.CompletedTask;
    }

    public Task ReplaceClaimAsync(IdentityUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
    {
        // Can be implemented to update claims in MongoDB
        return Task.CompletedTask;
    }

    public Task RemoveClaimsAsync(IdentityUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        // Can be implemented to remove claims from MongoDB
        return Task.CompletedTask;
    }

    public Task<IList<IdentityUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
    {
        // Can be implemented to query users by claim
        return Task.FromResult<IList<IdentityUser>>(new List<IdentityUser>());
    }

    public void Dispose()
    {
        // Nothing to dispose
    }
}

