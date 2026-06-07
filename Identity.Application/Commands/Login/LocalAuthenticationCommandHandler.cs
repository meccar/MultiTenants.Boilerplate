using BuildingBlocks.Shared.Dtos.Authentication;
using BuildingBlocks.Shared.Exceptions;
using BuildingBlocks.Shared.Helpers;
using BuildingBlocks.Shared.Models;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Tenancy.Domain.Interfaces;

namespace Identity.Application.Commands.Login;

public class LocalAuthenticationCommandHandler
    : IRequestHandler<LocalAuthenticationCommand, TokenDto>
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

    public async Task<TokenDto> Handle(
        LocalAuthenticationCommand request,
        CancellationToken cancellationToken)
    {
        //if (string.IsNullOrEmpty(_tenant.TenantId))
          //  throw new InvalidOperationException("Tenant context not available");

        var user = await _userManager.FindByNameAsync(
                       request.LoginDto.UserName)
            ?? await _userManager.FindByEmailAsync(
                request.LoginDto.UserName);
        if (user is null)
            throw new UnauthorizedException();

        var isValid = await _userManager.CheckPasswordAsync(
            user, 
            request.LoginDto.Password);
        if (!isValid)
            throw new UnauthorizedException();

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count == 0)
            throw new BadRequestException("User has no assigned roles");

        var tokenRequest = new GenerateTokenRequestModel(
            UserName: user.UserName ?? user.Email!,
            Roles: roles.ToList(),
            SecurityStamp: user.SecurityStamp!,
            TenantId: _tenant.TenantId
        );
        
        var tokenDto = await _jwtToken.GenerateJwtTokenAsync(tokenRequest);

        _logger.LogInformation(
            "User {UserId} successfully authenticated in tenant {TenantId}",
            user.Id, _tenant.TenantId);

        return tokenDto;
    }
}
