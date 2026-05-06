using Identity.Domain.Interfaces;
using Quartz;

namespace Identity.Infrastructure.Jobs;

public class QuartzJobEnqueuer : IJobEnqueuer
{
    private readonly ISchedulerFactory _schedulerFactory;

    public QuartzJobEnqueuer(ISchedulerFactory schedulerFactory)
        => _schedulerFactory = schedulerFactory;

    public async Task EnqueueAsync<TJob>(
        JobDataMap data,
        Action<TriggerBuilder>? configureTrigger = null,
        string group = "default",
        CancellationToken cancellationToken = default)
        where TJob : IJob
    {
        var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        var id = Guid.NewGuid().ToString();

        var job = JobBuilder.Create<TJob>()
            .WithIdentity($"{typeof(TJob).Name}-{id}", group)
            .UsingJobData(data)
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity($"{typeof(TJob).Name}-trigger-{id}", group);

        if (configureTrigger is null)
            trigger.StartNow();
        else
            configureTrigger(trigger);
        
        await scheduler.ScheduleJob(job, trigger.Build(), cancellationToken);
    }
}