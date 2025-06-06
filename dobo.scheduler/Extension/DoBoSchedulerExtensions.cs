using System.Reflection;
using dobo.core.Extensions;
using dobo.scheduler.Job;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;

namespace dobo.scheduler.Extension;

public static class DoBoSchedulerExtensions
{
    public static async Task<IServiceCollection> AddDoboScheduler(this IServiceCollection services, Assembly[] assemblies)
    {
        // var job = JobBuilder.Create<HelloJob>()
        //     .WithIdentity("job1", "group1")
        //     .Build();
        //
        // // Trigger the job to run now, and then repeat every 10 seconds
        // var trigger = TriggerBuilder.Create()
        //     .WithIdentity("trigger1", "group1")
        //     .StartNow()
        //     .WithSimpleSchedule(x => x
        //         .WithIntervalInSeconds(10)
        //         .RepeatForever())
        //     .Build();
        //
        // // Tell Quartz to schedule the job using our trigger
        // await scheduler.ScheduleJob(job, trigger);
        
        var handlers = assemblies.GetTypes(typeof(IJob));
        
        var factory = new StdSchedulerFactory();
        var scheduler = await factory.GetScheduler();
        services.AddSingleton(sp =>
        {
            var job = JobBuilder.Create(() => { })
                .WithIdentity("job1", "group1")
                .Build();
            
            return scheduler;
        });

        return services;
    }
}