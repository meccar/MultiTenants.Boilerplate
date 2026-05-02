using Identity.Domain.Interfaces;
using Identity.Domain.Records;
using Identity.Infrastructure.Jobs;
using Quartz;

public class PasswordResetEmailJob : IPasswordResetEmailJob
{
    private readonly ISchedulerFactory _schedulerFactory;

    public PasswordResetEmailJob(ISchedulerFactory schedulerFactory)
        => _schedulerFactory = schedulerFactory;

    // Enqueue logic moves here — out of the handler
    public async Task EnqueueAsync(string email, string callbackUrl, CancellationToken cancellationToken)
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

        var jobData = new JobDataMap
        {
            { nameof(SendPasswordResetEmailJobData.Email), email },
            { nameof(SendPasswordResetEmailJobData.CallbackUrl), callbackUrl }
        };

        var job = JobBuilder.Create<SendPasswordResetEmailQuartzJob>()
            .WithIdentity($"password-reset-{Guid.NewGuid()}", "identity")
            .UsingJobData(jobData)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"password-reset-trigger-{Guid.NewGuid()}", "identity")
            .StartNow()
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }

    public Task Execute(SendPasswordResetEmailJobData data, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

