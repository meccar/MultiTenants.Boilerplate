using Identity.Domain.Interfaces;
using Identity.Domain.Records;
using Quartz;

namespace Identity.Infrastructure.Jobs;

public class PasswordResetEmailJob : IPasswordResetEmailJob
{
    private readonly IJobEnqueuer _jobEnqueuer;

    public PasswordResetEmailJob(IJobEnqueuer jobEnqueuer)
        => _jobEnqueuer = jobEnqueuer;

    public Task EnqueueAsync(string email, string callbackUrl, CancellationToken cancellationToken)
    {
        var data = new JobDataMap
        {
            { nameof(SendPasswordResetEmailJobData.Email), email },
            { nameof(SendPasswordResetEmailJobData.CallbackUrl), callbackUrl }
        };

        return _jobEnqueuer.EnqueueAsync<SendPasswordResetEmailQuartzJob>(
            data,
            group: "identity",
            cancellationToken);
    }
}