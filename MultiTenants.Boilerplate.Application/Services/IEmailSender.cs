namespace MultiTenants.Boilerplate.Application.Services;

/// <summary>
/// Service for sending emails (e.g. confirmation, password reset).
/// Replace with a real implementation (SendGrid, SMTP, etc.) in production.
/// </summary>
public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string htmlMessage, CancellationToken cancellationToken = default);
}
