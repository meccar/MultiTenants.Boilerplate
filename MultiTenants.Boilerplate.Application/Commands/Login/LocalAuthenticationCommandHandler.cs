using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using MultiTenants.Boilerplate.Application.Abstractions;
using MultiTenants.Boilerplate.Application.Helpers;
using MultiTenants.Boilerplate.Shared.Constants.Errors;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands.Login;

public class LocalAuthenticationCommandHandler
    : IRequestHandler<LocalAuthenticationCommand, Result<string>>
{
    private readonly IIdentityService _identityService;
    private readonly ITenantProvider _tenantProvider;
    private readonly ILogger<LocalAuthenticationCommandHandler> _logger;
    private readonly JwtToken _jwtToken;

    public LocalAuthenticationCommandHandler(
        IIdentityService identityService,
        ILogger<LocalAuthenticationCommandHandler> logger,
        JwtToken jwtToken,
        ITenantProvider tenantProvider)
    {
        _identityService = identityService;
        _logger = logger;
        _jwtToken = jwtToken;
        _tenantProvider = tenantProvider;
    }

    public async Task<Result<string>> Handle(LocalAuthenticationCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.GetCurrentTenantId();
        if (string.IsNullOrEmpty(tenantId))
            return Result<string>.Failure("Tenant context not found");

        var user = await _identityService.GetUserByUserNameOrEmailAsync(request.UserName, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Login failed: User {UserName} not found in tenant {TenantId}",
                StringHelper.MaskInput(request.UserName), tenantId);
            return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);
        }

        if (!user.EmailConfirmed)
        {
            _logger.LogWarning("Login failed: User {UserId} email is not confirmed in tenant {TenantId}",
                user.Id, tenantId);
            return Result<string>.Failure("User email is not confirmed");
        }

        if (!await _identityService.ValidateCredentialsAsync(request.UserName, request.Password, cancellationToken))
        {
            _logger.LogWarning("Login failed: Invalid credentials for user in tenant {TenantId}", tenantId);
            return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);
        }

        var roles = await _identityService.GetUserRolesAsync(user.Id, cancellationToken);
        if (roles.Count == 0)
        {
            _logger.LogWarning("Login failed: User {UserId} has no roles in tenant {TenantId}",
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

        _logger.LogInformation("User {UserId} successfully authenticated in tenant {TenantId}",
            user.Id, tenantId);

        return Result<string>.Success(token);
    }
}
