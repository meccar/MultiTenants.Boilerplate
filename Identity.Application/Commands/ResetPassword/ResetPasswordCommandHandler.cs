using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.ResetPassword;

public class ResetPasswordCommandHandler 
    : IRequestHandler<ResetPasswordCommand, IdentityResult>
{
    private readonly ILogger<ResetPasswordCommandHandler> _logger;
    private readonly UserManager<UsersEntity> _userManager;

    public ResetPasswordCommandHandler(
        ILogger<ResetPasswordCommandHandler> logger,
        UserManager<UsersEntity> userManager
    ){
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IdentityResult> Handle(
        ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new UnauthorizedAccessException();
        
        var result = await _userManager.ResetPasswordAsync(
            user, request.Token, request.NewPassword);

        if (result.Succeeded)
        {
            _logger.LogInformation("Password reset successfully for {Email}.", request.Email);
            return result;
        }
        else
        {
            _logger.LogWarning("ResetPassword failed for {Email}: {Error}", request.Email, result.Errors);
            throw new ApplicationException();
        }
    }
}
