using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Shared.Utilities;

namespace BuildingBlocks.Application.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        IConfiguration configuration,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result> Handle(
        ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        // Always return Success to avoid email enumeration attacks
        var token = await _identityService.GeneratePasswordResetTokenAsync(request.Email, cancellationToken);
        if (token is null)
        {
            _logger.LogInformation("ForgotPassword: no user found for email (not disclosed to caller).");
            return Result.Success();
        }

        var baseUrl = _configuration["App:BaseUrl"]?.TrimEnd('/') ?? "https://localhost";
        var callbackUrl = $"{baseUrl}/reset-password?email={Uri.EscapeDataString(request.Email)}&token={Uri.EscapeDataString(token)}";

        await _emailSender.SendEmailAsync(
            request.Email,
            "Reset your password",
            $"Reset your password by visiting: {callbackUrl}",
            cancellationToken);

        _logger.LogInformation("Password reset link sent to {Email}.", request.Email);
        return Result.Success();
    }
}
