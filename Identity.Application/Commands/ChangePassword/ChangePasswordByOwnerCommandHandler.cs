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
    private readonly IUserStore<UsersEntity> _userStore;
    private readonly IPasswordHasher<UsersEntity> _passwordHasher;
    private readonly ILogger<ChangePasswordByOwnerCommandHandler> _logger;

    public ChangePasswordByOwnerCommandHandler(
        UserManager<UsersEntity> userManager,
        IUserStore<UsersEntity> userStore,
        IPasswordHasher<UsersEntity> passwordHasher,
        ILogger<ChangePasswordByOwnerCommandHandler> logger)
    {
        _userManager = userManager;
        _userStore = userStore;
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
        
        if (_userStore is not IUserPasswordStore<UsersEntity> passwordStore)
            return Result.Failure("User store does not support password changes");

        await passwordStore.SetPasswordHashAsync(
            user,
            hash,
            cancellationToken);

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            return Result.Failure(string.Join("; ", updateResult.Errors.Select(error => error.Description)));

        _logger.LogInformation("Password changed successfully for user {UserId}", 
            user.Id);
        return Result.Success();
    }
}
