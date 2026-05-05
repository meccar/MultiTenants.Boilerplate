public class QuartzJobEnqueuer : IJobEnqueuer
{
    private readonly ISchedulerFactory _schedulerFactory;

    public QuartzJobEnqueuer(ISchedulerFactory schedulerFactory)
        => _schedulerFactory = schedulerFactory;

    public async Task EnqueueAsync<TJob>(
        JobDataMap data,
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
            .WithIdentity($"{typeof(TJob).Name}-trigger-{id}", group)
            .StartNow()
            .Build();

        await scheduler.ScheduleJob(job, trigger, cancellationToken);
    }
}
