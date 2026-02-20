using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MultiTenants.Boilerplate.Infrastructure.Identity;

namespace MultiTenants.Boilerplate.Infrastructure.Services;

/// <summary>
/// Identity IEmailSender implementation that logs instead of sending.
/// Replace with a real implementation (SendGrid, SMTP) in production.
/// </summary>
internal sealed class LoggingIdentityEmailSender : IEmailSender<AppUser>
{
    private readonly ILogger<LoggingIdentityEmailSender> _logger;

    public LoggingIdentityEmailSender(ILogger<LoggingIdentityEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendConfirmationLinkAsync(AppUser user, string email, string confirmationLink)
    {
        _logger.LogInformation("Confirm email link would be sent to {Email}. Link: {Link}", email, confirmationLink);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(AppUser user, string email, string resetLink)
    {
        _logger.LogInformation("Password reset link would be sent to {Email}. Link: {Link}", email, resetLink);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(AppUser user, string email, string resetCode)
    {
        _logger.LogInformation("Password reset code would be sent to {Email}. Code: {Code}", email, resetCode);
        return Task.CompletedTask;
    }
}
