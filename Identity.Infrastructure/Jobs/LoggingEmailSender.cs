using BuildingBlocks.Shared.Helpers;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure.Jobs;

public sealed class LoggingEmailSender : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> _logger;

    public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        _logger.LogInformation(
            "Email queued for {Email}. Subject: {Subject}. Body: {Body}",
            StringHelper.MaskInput(email),
            subject,
            htmlMessage);

        return Task.CompletedTask;
    }
}
