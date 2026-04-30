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
    private readonly UserManager<UsersEntity> _userManager;
    private readonly ITenant _tenant;
    private readonly ILogger<LocalAuthenticationCommandHandler> _logger;
    private readonly JwtToken _jwtToken;

    public LocalAuthenticationCommandHandler(
        UserManager<UsersEntity> userManager,
        ITenant tenant,
        ILogger<LocalAuthenticationCommandHandler> logger,
        JwtToken jwtToken
    ){
        _userManager = userManager;
        _tenant = tenant;
        _logger = logger;
        _jwtToken = jwtToken;
    }

    public async Task<Result<string>> Handle(
        LocalAuthenticationCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_tenant.TenantId))
            throw new InvalidOperationException("Tenant context not available");

        var user = await _userManager.FindByNameAsync(
                       request.LoginDto.UserName)
            ?? await _userManager.FindByEmailAsync(
                request.LoginDto.UserName);
        if (user is null)
            return Result<string>.Failure(
                AuthenticationErrors.InvalidCredentials);

        var isValid = await _userManager.CheckPasswordAsync(
            user, 
            request.LoginDto.Password);
        if (!isValid)
            return Result<string>.Failure(
                AuthenticationErrors.InvalidCredentials);

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count == 0)
            return Result<string>.Failure("User has no assigned roles");

        var token = await _jwtToken.GenerateJwtTokenAsync(
            user.Email, roles.ToList(), _tenant.TenantId);
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
