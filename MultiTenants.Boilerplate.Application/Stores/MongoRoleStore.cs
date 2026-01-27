using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace MultiTenants.Boilerplate.Application.Stores;

public class MongoRoleStore : IRoleStore<IdentityRole>
{
  private readonly ILogger<MongoRoleStore> _logger;
    private readonly IMongoCollection<IdentityRole> _roles;

    public MongoRoleStore(IMongoDatabase database, ILogger<MongoRoleStore> logger)
    {
      _logger = logger;
        _roles = database.GetCollection<IdentityRole>("Roles");
    }

    public async Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        try
        {
            await _roles.InsertOneAsync(role, cancellationToken: cancellationToken);
            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role");
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }

    public async Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        try
        {
            await _roles.DeleteOneAsync(r => r.Id == role.Id, cancellationToken);
            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role");
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }

    public async Task<IdentityRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        return await _roles.Find(r => r.Id == roleId).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IdentityRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        return await _roles.Find(r => r.NormalizedName == normalizedRoleName.ToUpper()).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<string?> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.NormalizedName);
    }

    public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Id);
    }

    public Task<string?> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Name);
    }

    public Task SetNormalizedRoleNameAsync(IdentityRole role, string? normalizedName, CancellationToken cancellationToken)
    {
        role.NormalizedName = normalizedName;
        return Task.CompletedTask;
    }

    public Task SetRoleNameAsync(IdentityRole role, string? roleName, CancellationToken cancellationToken)
    {
        role.Name = roleName;
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        try
        {
            await _roles.ReplaceOneAsync(r => r.Id == role.Id, role, cancellationToken: cancellationToken);
            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role");
            return IdentityResult.Failed(new IdentityError { Description = ex.Message });
        }
    }

    public void Dispose()
    {
        // Nothing to dispose
    }
}

