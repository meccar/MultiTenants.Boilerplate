using Quartz;

namespace Identity.Infrastructure.Jobs;

public static class JobTriggers
{
    public static Action<TriggerBuilder> FireAndForget =>
        t => t.StartNow();

    public static Action<TriggerBuilder> Delayed(TimeSpan delay) =>
        t => t.StartAt(DateTimeOffset.UtcNow.Add(delay));

    public static Action<TriggerBuilder> Cron(string expression) =>
        t => t.WithCronSchedule(expression);

    public static Action<TriggerBuilder> At(DateTimeOffset when) =>
        t => t.StartAt(when);
}