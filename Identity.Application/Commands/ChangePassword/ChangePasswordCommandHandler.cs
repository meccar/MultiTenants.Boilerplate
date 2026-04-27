using MediatR;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Shared.Utilities;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Commands.ChangePassword;

public class ChangePasswordCommandHandler 
    : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly UserManager<UsersEntity> _userManager;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        UserManager<UsersEntity> userManager,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> Handle(
        ChangePasswordCommand request, 
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", request.UserId);
            return Result.Failure("User not found.");
        }

        // Verify current password
        var isPasswordValid = await _userManager.CheckPasswordAsync(
            user, request.CurrentPassword);
        if (!isPasswordValid)
        {
            _logger.LogWarning("Incorrect password for user {UserId}", request.UserId);
            return Result.Failure("Current password is incorrect.");
        }

        // Change password
        var result = await _userManager.ChangePasswordAsync(
            user,
            request.CurrentPassword,
            request.NewPassword);

        if (!result.Succeeded)
        {
            _logger.LogWarning("Failed to change password for user {UserId}", request.UserId);
            return Result.Failure($"Failed to change password: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        _logger.LogInformation("Password changed successfully for user {UserId}", request.UserId);
        return Result.Success();
    }
}
