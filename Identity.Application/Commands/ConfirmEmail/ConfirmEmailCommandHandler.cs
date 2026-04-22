using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler 
    : IRequestHandler<ConfirmEmailCommand, IdentityResult>
{
    private readonly ILogger<ConfirmEmailCommandHandler> _logger;
    private readonly UserManager<AppUser> _userManager;

    public ConfirmEmailCommandHandler(
        ILogger<ConfirmEmailCommandHandler> logger,
        UserManager<AppUser> userManager
    ){
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IdentityResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new UnauthorizedAccessException();
        
        var result = await _userManager.ConfirmEmailAsync(user, request.Token);

        if (!result.Succeeded)
            _logger.LogWarning("ConfirmEmail failed for user {UserId}: {Error}", request.Email, result.Errors);
        else
            _logger.LogInformation("Email confirmed for user {UserId}.", request.Email);

        return result;
    }
}
