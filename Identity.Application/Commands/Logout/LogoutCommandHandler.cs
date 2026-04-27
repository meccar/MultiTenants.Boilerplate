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
    private readonly SignInManager<UsersEntity> _signInManager;

    public LogoutCommandHandler(
        ILogger<LogoutCommandHandler> logger,
        SignInManager<UsersEntity> signInManager
    ){
        _logger = logger;
        _signInManager = signInManager;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User signed out.");
        return Result.Success();
    }
}
