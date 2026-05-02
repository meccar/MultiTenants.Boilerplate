using System.Text;
using BuildingBlocks.Shared.Constants;
using BuildingBlocks.Shared.Utilities;
using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler 
    : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;
    private readonly UserManager<UsersEntity> _userManager;
    private readonly IPasswordResetEmailJob _passwordResetEmailJob;

    public ForgotPasswordCommandHandler(
        IConfiguration configuration,
        ILogger<ForgotPasswordCommandHandler> logger,
        UserManager<UsersEntity> userManager,
        IPasswordResetEmailJob passwordResetEmailJob
    ){
        _configuration = configuration;
        _logger = logger;
        _userManager = userManager;
        _passwordResetEmailJob = passwordResetEmailJob;
    }

    public async Task<Result> Handle(
        ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user  = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result.Failure(ResponseMessageConstants.Unauthorized);
        }        
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        
        var baseUrl = _configuration["App:BaseUrl"]?.TrimEnd('/') 
                      ?? throw new InvalidOperationException($"{nameof(ForgotPasswordCommandHandler)}: BaseUrl is not configured");
        var callbackUrl = $"{baseUrl}/reset-password?token={encodedToken}";

        await _passwordResetEmailJob.EnqueueAsync(
            request.Email, callbackUrl, cancellationToken);

        _logger.LogInformation("Password reset link sent to {Email}.", request.Email);
        return Result.Success();
    }
}
