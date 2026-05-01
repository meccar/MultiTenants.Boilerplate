using BuildingBlocks.Shared.Constants.Errors;
using MediatR;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Shared.Utilities;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Commands.ChangePassword;

public class ChangePasswordByOwnerCommandHandler 
    : IRequestHandler<ChangePasswordByOwnerCommand, Result>
{
    private readonly UserManager<UsersEntity> _userManager;
    private readonly IUserPasswordStore<UsersEntity> _passwordStore;
    private readonly IPasswordHasher<UsersEntity> _passwordHasher;
    private readonly ILogger<ChangeLoggedInUserPasswordCommandHandler> _logger;

    public ChangePasswordByOwnerCommandHandler(
        UserManager<UsersEntity> userManager,
        IUserPasswordStore<UsersEntity> passwordStore,
        IPasswordHasher<UsersEntity> passwordHasher,
        ILogger<ChangeLoggedInUserPasswordCommandHandler> logger)
    {
        _userManager = userManager;
        _passwordStore = passwordStore;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<Result> Handle(
        ChangePasswordByOwnerCommand request, 
        CancellationToken cancellationToken)
    {
        if (request.ChangePasswordDto.UserName is null)
            return Result<string>.Failure("UserName is incorrect"); 
        
        var user = await _userManager.FindByNameAsync(
                       request.ChangePasswordDto.UserName)
                   ?? await _userManager.FindByEmailAsync(
                       request.ChangePasswordDto.UserName);
        
        if (user is null)
            return Result<string>.Failure(
                AuthenticationErrors.InvalidCredentials); 

        // TODO: verify user's tenant with owner's

        // Change password
        var hash = _passwordHasher.HashPassword(
            user, request.ChangePasswordDto.NewPassword);
        
        await _passwordStore.SetPasswordHashAsync(
            user,
            hash,
            cancellationToken);

        _logger.LogInformation("Password changed successfully for user {UserId}", 
            user.Id);
        return Result.Success();
    }
}