public interface IJobEnqueuer
{
    Task EnqueueAsync<TJob>(
        JobDataMap data,
        string group = "default",
        CancellationToken cancellationToken = default)
        where TJob : IJob;
}
