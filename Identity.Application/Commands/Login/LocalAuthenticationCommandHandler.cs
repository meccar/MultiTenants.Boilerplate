using BuildingBlocks.Shared.Constants.Errors;
using BuildingBlocks.Shared.Helpers;
using BuildingBlocks.Shared.Utilities;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Tenancy.Domain.Interfaces;

namespace Identity.Application.Commands.Login;

public class LocalAuthenticationCommandHandler
    : IRequestHandler<LocalAuthenticationCommand, Result<string>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ITenant _tenant;
    private readonly ILogger<LocalAuthenticationCommandHandler> _logger;
    private readonly JwtToken _jwtToken;

    public LocalAuthenticationCommandHandler(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<AppRole> roleManager,
        ITenant tenant,
        ILogger<LocalAuthenticationCommandHandler> logger,
        JwtToken jwtToken
    ){
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tenant = tenant;
        _logger = logger;
        _jwtToken = jwtToken;
    }

    public async Task<Result<string>> Handle(
        LocalAuthenticationCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_tenant.TenantId))
            throw new InvalidOperationException("Tenant context not available");

        var user = await _userManager.FindByNameAsync(request.UserName)
            ?? await _userManager.FindByEmailAsync(request.UserName);
        if (user is null)
            return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);

        var checkPasswordSignIn = await _signInManager.PasswordSignInAsync(user, request.Password, true, true);
        if (!checkPasswordSignIn.Succeeded)
            return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count == 0)
            return Result<string>.Failure("User has no assigned roles");

        var token = await _jwtToken.GenerateJwtTokenAsync(
            user.Id, user.Email, roles.ToList(), _tenant.TenantId);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogError("Token generation failed for user {UserId} in tenant {TenantId}",
                user.Id, _tenant.TenantId);
            return Result<string>.Failure("Token generation failed");
        }

        _logger.LogInformation("User {UserId} successfully authenticated in tenant {TenantId}",
            user.Id, _tenant.TenantId);

        return Result<string>.Success(token);
    }
}
