using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MultiTenants.Boilerplate.Application.Helpers;
using MultiTenants.Boilerplate.Domain.Abstractions;
using MultiTenants.Boilerplate.Shared.Constants.Errors;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands.Login;

public class LocalAuthenticationCommandHandler
    : IRequestHandler<LocalAuthenticationCommand, Result<string>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITenantRepository _tenantRepository;
    private readonly ILogger<LocalAuthenticationCommandHandler> _logger;
    private readonly JwtToken _jwtToken;

    public LocalAuthenticationCommandHandler(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        ITenantRepository tenantRepository,
        ILogger<LocalAuthenticationCommandHandler> logger,
        JwtToken jwtToken
    ){
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tenantRepository = tenantRepository;
        _logger = logger;
        _jwtToken = jwtToken;
    }

    public async Task<Result<string>> Handle(LocalAuthenticationCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(Guid.Parse(request.TenantId));
        if (tenant is null)
            return Result<string>.Failure("Tenant context not found");

        var user = await _userManager.FindByNameAsync(request.UserName)
            ?? await _userManager.FindByEmailAsync(request.UserName);
        if (user is null)
        {
            //_logger.LogWarning("Login failed: User {UserName} not found in tenant {TenantId}",
            //    StringHelper.MaskInput(request.UserName), tenantId);
            return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);
        }

        var checkPasswordSignIn = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (!checkPasswordSignIn.Succeeded)
            return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count == 0)
            return Result<string>.Failure("User has no assigned roles");

        var token = await _jwtToken.GenerateJwtTokenAsync(user, roles.ToList(), request.TenantId);
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogError("Token generation failed for user {UserId} in tenant {TenantId}",
                user.Id, request.TenantId);
            return Result<string>.Failure("Token generation failed");
        }

        _logger.LogInformation("User {UserId} successfully authenticated in tenant {TenantId}",
            user.Id, request.TenantId);

        return Result<string>.Success(token);
    }
}
