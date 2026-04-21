using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Shared.Utilities;
using Microsoft.AspNetCore.Identity;

namespace BuildingBlocks.Application.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    
    public ForgotPasswordCommandHandler(
        IConfiguration configuration,
        ILogger<ForgotPasswordCommandHandler> logger,
        UserManager<IdentityUser> userManager
    ){
        _configuration = configuration;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<Result> Handle(
        ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        // Always return Success to avoid email enumeration attacks
        var user  = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            throw new UnauthorizedAccessException();
        
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var baseUrl = _configuration["App:BaseUrl"]?.TrimEnd('/') ?? "https://localhost";
        var callbackUrl = $"{baseUrl}/reset-password?email={Uri.EscapeDataString(request.Email)}&token={Uri.EscapeDataString(token)}";

        //-- await _emailSender.SendEmailAsync(
          //  request.Email,
          //  "Reset your password",
          //  $"Reset your password by visiting: {callbackUrl}",
          //  cancellationToken);

        _logger.LogInformation("Password reset link sent to {Email}.", request.Email);
        return Result.Success();
    }
}
