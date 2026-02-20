using Microsoft.Extensions.Logging;

namespace MultiTenants.Boilerplate.Application.Services;

/// <summary>
/// Development email sender that logs the email content instead of sending.
/// Replace with a real IEmailSender implementation in production.
/// </summary>
public class LoggingEmailSender : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> _logger;

    public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Email would be sent to {Email}. Subject: {Subject}. Body: {Body}",
            email, subject, htmlMessage);
        return Task.CompletedTask;
    }
}
