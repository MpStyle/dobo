using System.Reflection;
using dobo.core.Extensions;
using dobo.telegram.Extension;
using dobo.TelegramCommand;
using dobo.waste.collection;
using dobo.waste.collection.Entities;
using dobo.waste.collection.MessageBuilder;
using dobo.waste.collection.Padova;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace dobo.Extension;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Init(this IServiceCollection services)
    {
        services
            .AddTelegramCommandHandlers(Assembly.GetAssembly(typeof(HelpCommand)))
            .AddTelegramBotClient()
            .AddSingleton<GarbageMessageBuilder>()
            .AddGarbageScraper();
        
        services.AddQuartz();
        services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });

        return services;
    }
}