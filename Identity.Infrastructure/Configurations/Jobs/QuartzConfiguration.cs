using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Identity.Infrastructure.Configurations.Jobs;

public static class QuartzConfiguration
{
    public static IServiceCollection AddQuartzInfrastructure(
        this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            q.UseSimpleTypeLoader();
            q.UseInMemoryStore();
            q.UseDefaultThreadPool(tp =>
                tp.MaxConcurrency = 10);
        });

        services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });

        return services;
    }
}