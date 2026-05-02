using BuildingBlocks.Application.Helpers;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Identity.Infrastructure.Jobs;

public class SendPasswordResetEmailQuartzJob : IJob
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<SendPasswordResetEmailQuartzJob> _logger;

    public SendPasswordResetEmailQuartzJob(
        IEmailSender emailSender,
        ILogger<SendPasswordResetEmailQuartzJob> logger
    ){
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var data = context.JobDetail.JobDataMap;
        var email = data.GetString("Email")!;
        var callbackUrl = data.GetString("CallbackUrl")!;

        try
        {
            // SendEmailAsync(email, subject, htmlMessage) — 3 params, no CancellationToken
            await _emailSender.SendEmailAsync(
                email,
                "Reset your password",
                $"Reset your password by visiting: {callbackUrl}");

            _logger.LogInformation("Password reset email sent to {Email}",
                StringHelper.MaskInput(email));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}",
                StringHelper.MaskInput(email));
            throw new JobExecutionException(ex, refireImmediately: false);
        }
    }
}