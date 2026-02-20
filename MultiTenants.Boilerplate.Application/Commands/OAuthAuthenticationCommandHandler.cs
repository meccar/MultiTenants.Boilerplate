using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MultiTenants.Boilerplate.Application.Abstractions;
using MultiTenants.Boilerplate.Application.Helpers;
using MultiTenants.Boilerplate.Shared.Constants.Errors;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands;

public class OAuthAuthenticationCommandHandler
    : IRequestHandler<OAuthAuthenticationCommand, Result<string>>
{
    private readonly IIdentityService _identityService;
    private readonly ITenantProvider _tenantProvider;
    private readonly IMultiTenantContextAccessor<TenantInfo> _tenantContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OAuthAuthenticationCommandHandler> _logger;
    private readonly JwtToken _jwtToken;

    private const string DefaultOAuthRole = "User";

    public OAuthAuthenticationCommandHandler(
        IIdentityService identityService,
        IMultiTenantContextAccessor<TenantInfo> tenantContextAccessor,
        IConfiguration configuration,
        ILogger<OAuthAuthenticationCommandHandler> logger,
        JwtToken jwtToken,
        ITenantProvider tenantProvider)
    {
        _identityService = identityService;
        _tenantContextAccessor = tenantContextAccessor;
        _configuration = configuration;
        _logger = logger;
        _jwtToken = jwtToken;
        _tenantProvider = tenantProvider;
    }

    public async Task<Result<string>> Handle(OAuthAuthenticationCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.GetCurrentTenantId();
        if (string.IsNullOrEmpty(tenantId))
            return Result<string>.Failure("Tenant context not found");

        var email = request.Email;
        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("OAuth login failed: no email in external claims");
            return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);
        }

        var normalizedEmail = email.Trim().ToUpperInvariant();
        var user = await _identityService.GetUserByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Login failed: User {UserName} not found in tenant {TenantId}",
                StringHelper.MaskInput(email), tenantId);
            return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);
        }

        var roles = await _identityService.GetUserRolesAsync(user.Id, cancellationToken);
        if (roles.Count == 0)
        {
            var defaultRole = _configuration["Authentication:OAuth:DefaultRole"] ?? DefaultOAuthRole;
            var assignResult = await _identityService.AssignRoleAsync(user.Id, defaultRole, cancellationToken);
            if (assignResult.IsSuccess)
                roles = await _identityService.GetUserRolesAsync(user.Id, cancellationToken);
        }

        if (roles.Count == 0)
        {
            _logger.LogWarning("OAuth login failed: User {UserId} has no roles in tenant {TenantId}",
                user.Id, tenantId);
            return Result<string>.Failure("User has no assigned roles");
        }

        var token = await _jwtToken.GenerateJwtTokenAsync(user, roles.ToList(), tenantId);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogError("Token generation failed for user {UserId} in tenant {TenantId}",
                user.Id, tenantId);
            return Result<string>.Failure("Token generation failed");
        }

        _logger.LogInformation("User {UserId} successfully authenticated via {Provider} in tenant {TenantId}",
            user.Id, request.Provider, tenantId);

        return Result<string>.Success(token);
    }
}
