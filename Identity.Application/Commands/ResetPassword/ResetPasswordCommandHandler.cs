using System.Text;
using BuildingBlocks.Shared.Constants.Errors;
using BuildingBlocks.Shared.Utilities;
using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.ResetPassword;

public class ResetPasswordCommandHandler 
    : IRequestHandler<ResetPasswordCommand, Result>
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

    public async Task<Result> Handle(
        ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var decodedToken = Encoding.UTF8.GetString(
            WebEncoders.Base64UrlDecode(request.Token));
        
        var user = await _userManager.FindByNameAsync(
                       request.ResetPasswordDto.UserName)
                   ?? await _userManager.FindByEmailAsync(
                       request.ResetPasswordDto.UserName);        
        if (user == null)
            return Result.Failure(
                AuthenticationErrors.InvalidCredentials);
        
        var result = await _userManager.ResetPasswordAsync(
            user, decodedToken, request.ResetPasswordDto.NewPassword);

        if (result.Succeeded)
        {
            _logger.LogInformation("Password reset successfully for {Email}.", request.ResetPasswordDto.UserName);
            return Result.Success();
        }
        
        _logger.LogWarning("ResetPassword failed for {Email}: {Error}", request.ResetPasswordDto.UserName, result.Errors);
        return Result.Failure(
            AuthenticationErrors.InvalidCredentials);
    }
}
