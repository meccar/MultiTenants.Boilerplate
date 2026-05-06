using Quartz;

namespace Identity.Domain.Interfaces;

public interface IJobEnqueuer
{
    Task EnqueueAsync<TJob>(
        JobDataMap data,
        Action<TriggerBuilder>? configureTrigger = null,
        string group = "default",
        CancellationToken cancellationToken = default)
        where TJob : IJob;
}