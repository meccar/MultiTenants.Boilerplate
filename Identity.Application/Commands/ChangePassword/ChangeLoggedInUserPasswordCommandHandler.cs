using MediatR;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Shared.Utilities;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Commands.ChangePassword;

public class ChangeLoggedInUserPasswordCommandHandler 
    : IRequestHandler<ChangeLoggedInUserPasswordCommand, Result>
{
    private readonly UserManager<UsersEntity> _userManager;
    private readonly ILogger<ChangeLoggedInUserPasswordCommandHandler> _logger;

    public ChangeLoggedInUserPasswordCommandHandler(
        UserManager<UsersEntity> userManager,
        ILogger<ChangeLoggedInUserPasswordCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> Handle(
        ChangeLoggedInUserPasswordCommand request, 
        CancellationToken cancellationToken)
    {
        if (request.ChangePasswordDto.CurrentPassword is null)
            return Result.Failure("Current password is incorrect.");
        
        // Verify current password
        var isPasswordValid = await _userManager.CheckPasswordAsync(
            request.CurrentUser.User, 
            request.ChangePasswordDto.CurrentPassword);
        if (!isPasswordValid)
        {
            _logger.LogWarning("Incorrect password for user {UserId}", request.CurrentUser.User.Id);
            return Result.Failure("Current password is incorrect.");
        }

        // Change password
        var result = await _userManager.ChangePasswordAsync(
            request.CurrentUser.User,
            request.ChangePasswordDto.CurrentPassword,
            request.ChangePasswordDto.NewPassword);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed to change password for user {UserId}", 
                request.CurrentUser.User.Id);
            return Result.Failure($"Failed to change password: {string.Join(", ", 
                result.Errors.Select(e => e.Description))}");
        }

        _logger.LogInformation("Password changed successfully for user {UserId}", 
            request.CurrentUser.User.Id);
        return Result.Success();
    }
}
