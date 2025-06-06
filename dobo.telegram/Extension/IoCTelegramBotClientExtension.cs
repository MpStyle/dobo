using System.Reflection;
using dobo.core.Extensions;
using dobo.telegram.Command;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using MessageHandler = dobo.telegram.Book.MessageHandler;

namespace dobo.telegram.Extension;

public static class IoCTelegramBotClientExtension
{
    public static IServiceCollection AddTelegramBotClient(this IServiceCollection services)
    {
        services
            .AddSingleton(sp =>
            {
                var token = sp.GetRequiredService<IConfiguration>().GetString("telegram:token");
                var bot = new TelegramBotClient(token);

                var commands = sp.GetServices<ITelegramCommandHandler>();
                var helpCommand = sp.GetService<IHelpCommandHandler>();
                var telegramCommands = commands.Select(c => new BotCommand
                {
                    Command = c.Command,
                    Description = c.Description
                });

                var telegramCommandList = telegramCommands.ToList();
                telegramCommandList.Add(new BotCommand
                {
                    Command = helpCommand.Command,
                    Description = helpCommand.Description
                });

                bot.SetMyCommands(telegramCommandList);

                return bot;
            })
            .AddSingleton<MessageHandler>();

        return services;
    }

    public static IServiceCollection AddTelegramCommandHandler(this IServiceCollection services, Assembly assembly)
    {
        return services.AddTelegramCommandHandler([assembly]);
    }

    public static IServiceCollection AddTelegramCommandHandler(this IServiceCollection services, Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            services.InitServiceCollection(assembly, typeof(ITelegramCommandHandler));
        }

        return services;
    }

    private static IServiceCollection InitServiceCollection(this IServiceCollection services,
        Assembly containerAssembly, Type serviceType)
    {
        var fullname = serviceType.FullName;
        if (fullname.IsNullOrEmpty())
        {
            return services;
        }

        var handlers = GetHandlers(containerAssembly, serviceType);
        var helpCommandHandlerExists = false;

        foreach (var handler in handlers)
        {
            if (typeof(IHelpCommandHandler).IsAssignableFrom(handler) && helpCommandHandlerExists == false)
            {
                services.AddSingleton(typeof(IHelpCommandHandler), handler);
                helpCommandHandlerExists = true;
            }
            else
            {
                services.Add(new ServiceDescriptor(serviceType, handler, ServiceLifetime.Singleton));
            }
        }

        return services;
    }

    private static Type[] GetHandlers(Assembly containerAssembly, Type serviceType)
    {
        var fullname = serviceType.FullName;
        if (fullname.IsNullOrEmpty())
        {
            return [];
        }

        var handlers = containerAssembly
            .GetExportedTypes()
            .Where(type => type.IsClass && type is {IsAbstract: false, IsGenericType: false, IsNested: false} &&
                           IsDerivedFromAbstractBase(type, fullname))
            .ToArray();

        return handlers;
    }

    private static bool IsDerivedFromAbstractBase(Type type, string baseFullName)
    {
        return type.FullName == baseFullName || type.GetInterfaces().Any(t => t.FullName == baseFullName);
    }
}