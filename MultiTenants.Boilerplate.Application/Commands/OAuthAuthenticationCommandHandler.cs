using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MultiTenants.Boilerplate.Application.Helpers;
using MultiTenants.Boilerplate.Shared.Constants.Errors;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands;

public class OAuthAuthenticationCommandHandler
    : IRequestHandler<OAuthAuthenticationCommand, Result<string>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMultiTenantContextAccessor<TenantInfo> _tenantContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OAuthAuthenticationCommandHandler> _logger;
    private readonly JwtToken _jwtToken;

    private const string DefaultOAuthRole = "User";

    public OAuthAuthenticationCommandHandler(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IMultiTenantContextAccessor<TenantInfo> tenantContextAccessor,
        IConfiguration configuration,
        ILogger<OAuthAuthenticationCommandHandler> logger,
        JwtToken jwtToken)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tenantContextAccessor = tenantContextAccessor;
        _configuration = configuration;
        _logger = logger;
        _jwtToken = jwtToken;
    }

    public async Task<Result<string>> Handle(OAuthAuthenticationCommand request, CancellationToken cancellationToken)
    {
        var tenant = _tenantContextAccessor.MultiTenantContext.TenantInfo;
        if (tenant is null)
        {
            _logger.LogWarning("OAuth login attempted without tenant context");
            return Result<string>.Failure("Tenant context not found");
        }

        if (tenant.Id != request.TenantId)
        {
            _logger.LogWarning("OAuth tenant mismatch: requested {Requested}, context {Context}",
                request.TenantId, tenant.Id);
            return Result<string>.Failure("Tenant mismatch");
        }

        var email = request.Email;
        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("OAuth login failed: no email in external claims");
            return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);
        }

        var user = await GetUserAsync(email);
        if (user is null)
        {
            _logger.LogWarning("Login failed: User {UserName} not found in tenant {TenantId}",
                StringHelper.MaskInput(email), tenant.Id);
            return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count == 0)
        {
            await EnsureDefaultRoleAndAssignAsync(user);
            roles = await _userManager.GetRolesAsync(user);
        }

        if (roles.Count == 0)
        {
            _logger.LogWarning("OAuth login failed: User {UserId} has no roles in tenant {TenantId}",
                user.Id, tenant.Id);
            return Result<string>.Failure("User has no assigned roles");
        }

        var token = await _jwtToken.GenerateJwtTokenAsync(user, roles, tenant);
        if (token == null)
        {
            _logger.LogError("Token generation failed for user {UserId} in tenant {TenantId}",
                user.Id, tenant.Id);
            return Result<string>.Failure("Token generation failed");
        }

        _logger.LogInformation("User {UserId} successfully authenticated via {Provider} in tenant {TenantId}",
            user.Id, request.Provider, tenant.Id);

        return Result<string>.Success(token);
    }

    private async Task<IdentityUser?> GetUserAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return null;

        var claims = await _userManager.GetClaimsAsync(user);
        var userTenantId = claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;
        if (userTenantId == null 
        || userTenantId != _tenantContextAccessor.MultiTenantContext.TenantInfo?.Id)
            return null;

        return user;
    }

    private async Task EnsureDefaultRoleAndAssignAsync(IdentityUser user)
    {
        var defaultRole = _configuration["Authentication:OAuth:DefaultRole"] ?? DefaultOAuthRole;

        if (await _roleManager.FindByNameAsync(defaultRole) == null)
            await _roleManager.CreateAsync(new IdentityRole(defaultRole));

        if (!(await _userManager.GetRolesAsync(user)).Contains(defaultRole))
            await _userManager.AddToRoleAsync(user, defaultRole);
    }
}
