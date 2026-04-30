using BuildingBlocks.Application.Helpers;
using BuildingBlocks.Shared.Constants.Errors;
using BuildingBlocks.Shared.Dtos;
using BuildingBlocks.Shared.Helpers;
using BuildingBlocks.Shared.Utilities;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tenancy.Domain.Interfaces;

namespace Identity.Application.Commands.Login;

public class OAuthAuthenticationCommandHandler
    : IRequestHandler<OAuthAuthenticationCommand, Result<string>>
{
    private readonly UserManager<UsersEntity> _userManager;
    private readonly ITenant _tenant;
    private readonly ILogger<OAuthAuthenticationCommandHandler> _logger;
    private readonly JwtToken _jwtToken;

    public OAuthAuthenticationCommandHandler(
        UserManager<UsersEntity> userManager,
        ITenant tenant,
        ILogger<OAuthAuthenticationCommandHandler> logger,
        JwtToken jwtToken
    ){
        _userManager = userManager;
        _tenant = tenant;
        _logger = logger;
        _jwtToken = jwtToken;
    }

    public async Task<Result<string>> Handle(
        OAuthAuthenticationCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_tenant.TenantId))
            throw new InvalidOperationException("Tenant context not available");
        var user = await _userManager.FindByLoginAsync(
            request.LoginDto.Provider, request.LoginDto.ProviderKey);
        if (user is null)
        {
            _logger.LogWarning("Login failed: User not found in tenant {TenantId}",
                _tenant.TenantId);
            return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count == 0)
        {
            _logger.LogWarning("Login failed: User has no role in tenant {TenantId}",
                _tenant.TenantId);
            return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);
        }

        var userDto = new UserDto
        {
            Email = user.Email!,
            UserName = user.UserName!,
            EmailConfirmed = user.EmailConfirmed,
        };
        var token = await _jwtToken.GenerateJwtTokenAsync(
            userDto, roles, _tenant.TenantId);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogError("Token generation failed for user {UserId} in tenant {TenantId}",
                user.Id, _tenant.TenantId);
            return Result<string>.Failure("Token generation failed");
        }

        _logger.LogInformation("User {UserId} successfully authenticated via {Provider} in tenant {TenantId}",
            user.Id, request.LoginDto.Provider, _tenant.TenantId);

        return Result<string>.Success(token);
    }
}
