using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MultiTenants.Boilerplate.Application.Abstractions;
using MultiTenants.Boilerplate.Application.Services;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands.ResendEmailConfirmation;

public class ResendEmailConfirmationCommandHandler : IRequestHandler<ResendEmailConfirmationCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ResendEmailConfirmationCommandHandler> _logger;

    public ResendEmailConfirmationCommandHandler(
        IIdentityService identityService,
        IEmailSender emailSender,
        IConfiguration configuration,
        ILogger<ResendEmailConfirmationCommandHandler> logger)
    {
        _identityService = identityService;
        _emailSender = emailSender;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result> Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var tokenResult = await _identityService.GenerateEmailConfirmationTokenAsync(request.Email, cancellationToken);
        if (tokenResult.IsFailure)
        {
            // "User not found" — respond with generic success to avoid enumeration
            if (tokenResult.Error == "User not found.")
            {
                _logger.LogInformation("ResendEmailConfirmation: user not found (not disclosed).");
                return Result.Success();
            }
            // "Email is already confirmed" — surface this error
            return tokenResult;
        }

        // token format: "userId|confirmationToken"
        var parts = tokenResult.Value!.Split('|', 2);
        var userId = parts[0];
        var code = parts[1];

        var baseUrl = _configuration["App:BaseUrl"]?.TrimEnd('/') ?? "https://localhost";
        var callbackUrl = $"{baseUrl}/confirm-email?userId={Uri.EscapeDataString(userId)}&code={Uri.EscapeDataString(code)}";

        await _emailSender.SendEmailAsync(
            request.Email,
            "Confirm your email",
            $"Please confirm your account by visiting: {callbackUrl}",
            cancellationToken);

        _logger.LogInformation("Email confirmation resent to {Email}.", request.Email);
        return Result.Success();
    }
}
