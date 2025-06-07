using System.Reflection;
using dobo.core.Extensions;
using dobo.info.Extension;
using dobo.info.Garbage.Entities;
using dobo.telegram.Command;
using dobo.telegram.Extension;
using dobo.waste.collection;
using dobo.waste.collection.Entities;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace dobo.Extension;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Init(this IServiceCollection services)
    {
        services
            .AddTelegramBotClient()
            .AddTelegramCommandHandlers()
            .AddGarbageScraper()
            .AddMessageBuilder();
        
        services.AddQuartz();
        services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });

        return services;
    }
}