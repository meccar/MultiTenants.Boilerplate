using Identity.Domain.Interfaces;
using Identity.Infrastructure.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Identity.Infrastructure.Configurations.Jobs;

public static class QuartzConfiguration
{
    [Obsolete("Obsolete")]
    public static IServiceCollection AddQuartzInfrastructure(
        this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.UseInMemoryStore();
            q.UseDefaultThreadPool(tp =>
                tp.MaxConcurrency = 10);
        });

        services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
            opt.AwaitApplicationStarted = true;
        });

        services.AddScoped<IJobEnqueuer, QuartzJobEnqueuer>();
        services.AddScoped<IPasswordResetEmailJob, PasswordResetEmailJob>();
        services.AddTransient<SendPasswordResetEmailQuartzJob>();
        
        return services;
    }
}