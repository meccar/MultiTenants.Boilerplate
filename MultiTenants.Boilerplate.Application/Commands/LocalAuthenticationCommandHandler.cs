using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MultiTenants.Boilerplate.Shared.Constants.Errors;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Helpers;

public class LocalAuthenticationCommandHandler 
    : IRequestHandler<LocalAuthenticationCommand, Result<string>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IMultiTenantContextAccessor<TenantInfo> _tenantContextAccessor;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LocalAuthenticationCommandHandler> _logger;
    private readonly JwtToken _jwtToken;

    public LocalAuthenticationCommandHandler(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IMultiTenantContextAccessor<TenantInfo> tenantContextAccessor,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        ILogger<LocalAuthenticationCommandHandler> logger,
        JwtToken jwtToken
    ){
        _userManager = userManager;
        _signInManager = signInManager;
        _tenantContextAccessor = tenantContextAccessor;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _logger = logger;
        _jwtToken = jwtToken;
    }

    public async Task<Result<string>> Handle(LocalAuthenticationCommand request, CancellationToken cancellationToken)
    {
        var tenant = _tenantContextAccessor.MultiTenantContext.TenantInfo;
        if (tenant is null)
        { 
            _logger.LogWarning("Login attempted without tenant context");
            return Result<string>.Failure("Tenant context not found");
        }

        if (tenant.Id != request.TenantId)
        {
            _logger.LogWarning("OAuth tenant mismatch: requested {Requested}, context {Context}",
                request.TenantId, tenant.Id);
            return Result<string>.Failure("Tenant mismatch");
        }

        var user = await GetUserAsync(request.UserName);
        if(user is null)
        {
            _logger.LogWarning("Login failed: User {UserName} not found in tenant {TenantId}",
                StringHelper.MaskInput(request.UserName), tenant.Id);
            return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);
        }

        if (!await _userManager.IsEmailConfirmedAsync(user))
        { 
            _logger.LogWarning("Login failed: User {UserId} email is not confirmed in tenant {TenantId}", 
                user.Id, tenant.Id);
            return Result<string>.Failure("User email is not confirmed");
        }

        var result = await _signInManager.PasswordSignInAsync(
            user.UserName!,
            request.Password,
            isPersistent: request.IsPersistent,
            lockoutOnFailure: true
        );

        if (result.IsLockedOut)
            return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);

        if (result.RequiresTwoFactor)
            return Result<string>.Failure("2FA_REQUIRED");

        if (!result.Succeeded)
            return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count == 0)
        {
            _logger.LogWarning("Login failed: User {UserId} has no roles in tenant {TenantId}", 
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

        _logger.LogInformation("User {UserId} successfully authenticated in tenant {TenantId}", 
            user.Id, tenant.Id);

        return Result<string>.Success(token);
    }

    private async Task<IdentityUser?> GetUserAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName)
            ?? await _userManager.FindByEmailAsync(userName);

        if (user == null)
            return null;

        var claims = await _userManager.GetClaimsAsync(user);
        var userTenantId = claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;

        if (userTenantId == null 
        || userTenantId != _tenantContextAccessor.MultiTenantContext.TenantInfo?.Id)
            return null;

        return user;
    }
}
