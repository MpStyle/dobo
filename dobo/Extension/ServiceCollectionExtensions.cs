using System.Reflection;
using dobo.MessageBuilder;
using dobo.telegram.Extension;
using dobo.TelegramCommand;
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
            .AddSingleton<GarbageMessageBuilder>();
        
        services.AddQuartz();
        services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
        });

        return services;
    }
}