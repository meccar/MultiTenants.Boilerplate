using BuildingBlocks.Shared.Constants.Errors;
using MediatR;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Shared.Utilities;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Commands.ChangePassword;

public class ChangeLoggedOutUserPasswordCommandHandler 
    : IRequestHandler<ChangeLoggedOutUserPasswordCommand, Result>
{
    private readonly UserManager<UsersEntity> _userManager;
    private readonly ILogger<ChangeLoggedInUserPasswordCommandHandler> _logger;

    public ChangeLoggedOutUserPasswordCommandHandler(
        UserManager<UsersEntity> userManager,
        ILogger<ChangeLoggedInUserPasswordCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> Handle(
        ChangeLoggedOutUserPasswordCommand request, 
        CancellationToken cancellationToken)
    {
        if (request.ChangePasswordDto.UserName is null)
            return Result.Failure("UserName is incorrect.");
        
        if (request.ChangePasswordDto.CurrentPassword is null)
            return Result.Failure("Current password is incorrect.");
            
        var user = await _userManager.FindByNameAsync(
                       request.ChangePasswordDto.UserName)
                   ?? await _userManager.FindByEmailAsync(
                       request.ChangePasswordDto.UserName);
        if (user is null)
            return Result<string>.Failure(
                AuthenticationErrors.InvalidCredentials); 
        
        // Verify current password
        var isPasswordValid = await _userManager.CheckPasswordAsync(
            user, 
            request.ChangePasswordDto.CurrentPassword);
        if (!isPasswordValid)
        {
            _logger.LogWarning("Incorrect password for user {UserId}", user.Id);
            return Result.Failure("Current password is incorrect.");
        }

        // Change password
        var result = await _userManager.ChangePasswordAsync(
            user,
            request.ChangePasswordDto.CurrentPassword,
            request.ChangePasswordDto.NewPassword);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed to change password for user {UserId}", 
                user.Id);
            return Result.Failure($"Failed to change password: {string.Join(", ", 
                result.Errors.Select(e => e.Description))}");
        }

        _logger.LogInformation("Password changed successfully for user {UserId}", 
            user.Id);
        return Result.Success();
    }
}