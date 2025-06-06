using dobo.QuartzJob;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace dobo.Extension;

public static class HostExtensions
{
    public static async Task AddGarbageJob(this IHost host)
    {
        var schedulerFactory = host.Services.GetRequiredService<ISchedulerFactory>();
        var scheduler = await schedulerFactory.GetScheduler();

        const string garbageJobName = nameof(GarbageJob);
        var job = JobBuilder.Create<GarbageJob>()
            .WithIdentity(garbageJobName, $"{garbageJobName}Group")
            .Build();

        // Trigger the job to run now, and then every 40 seconds
        var trigger = TriggerBuilder.Create()
            .WithIdentity($"{garbageJobName}Trigger", $"{garbageJobName}TriggerGroup")
            .StartNow()
            .WithCronSchedule("15 19 * * *")
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }
}