using BuildingBlocks.Shared.Utilities;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.Logout;

public class LogoutCommandHandler 
    : IRequestHandler<LogoutCommand, Result>
{
    private readonly ILogger<LogoutCommandHandler> _logger;
    private readonly UserManager<UsersEntity> _userManager;

    public LogoutCommandHandler(
        ILogger<LogoutCommandHandler> logger,
        UserManager<UsersEntity> userManager
    ){
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<Result> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _userManager.UpdateSecurityStampAsync(request.CurrentUser.User);
        if (!result.Succeeded)
        {
            _logger.LogError("Failed to invalidate token for user {UserId}", 
                request.CurrentUser.User.Id);
            return Result.Failure("Logout failed");
        }
        
        _logger.LogInformation("User {UserId} signed out successfully", 
            request.CurrentUser.User.Id);
        
        return Result.Success();
    }
}
