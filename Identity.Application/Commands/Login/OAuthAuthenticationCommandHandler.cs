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
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITenant _tenant;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OAuthAuthenticationCommandHandler> _logger;
    private readonly JwtToken _jwtToken;

    private const string DefaultOAuthRole = "User";

    public OAuthAuthenticationCommandHandler(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        SignInManager<AppUser> signInManager,
        ITenant tenant,
        IConfiguration configuration,
        ILogger<OAuthAuthenticationCommandHandler> logger,
        JwtToken jwtToken
    ){
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _tenant = tenant;
        _configuration = configuration;
        _logger = logger;
        _jwtToken = jwtToken;
    }

    public async Task<Result<string>> Handle(OAuthAuthenticationCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_tenant.TenantId))
            throw new InvalidOperationException("Tenant context not available");

        var email = request.Email;
        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogWarning("OAuth login failed: no email in external claims");
            return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            _logger.LogWarning("Login failed: User {UserName} not found in tenant {TenantId}",
                StringHelper.MaskInput(email), _tenant.TenantId);
            return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count == 0)
        {
            var defaultRole = _configuration["Authentication:OAuth:DefaultRole"] ?? DefaultOAuthRole;
            var assignResult = await _userManager.AddToRoleAsync(user, defaultRole);
            if (!assignResult.Succeeded)
            {
                _logger.LogWarning("OAuth login failed: User {UserId} has no roles in tenant {TenantId}",
                    user.Id, _tenant.TenantId);
                return Result<string>.Failure("User has no assigned roles");
            }
            roles = new List<string> { defaultRole };
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
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
            user.Id, request.Provider, _tenant.TenantId);

        return Result<string>.Success(token);
    }
}
