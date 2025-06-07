using dobo.core.Book;
using dobo.core.Extensions;
using dobo.telegram.Command;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using MessageHandler = dobo.telegram.Book.MessageHandler;

namespace dobo.telegram.Extension;

public static class IoCTelegramBotExtensions
{
    public static IServiceCollection AddTelegramBotClient(this IServiceCollection services)
    {
        services
            .AddSingleton(sp =>
            {
                var token = sp.GetRequiredService<IConfiguration>().GetString(AppSettingsKey.TelegramToken);
                if (token.IsNullOrEmpty())
                {
                    throw new Exception("Telegram token not configured");
                }
                
                var bot = new TelegramBotClient(token);

                var commands = sp.GetServices<ITelegramCommandHandler>();
                var helpCommand = sp.GetService<IHelpCommandHandler>();
                var telegramCommands = commands.Select(c => new BotCommand
                {
                    Command = c.Command,
                    Description = c.Description
                });

                var telegramCommandList = telegramCommands.ToList();
                if (helpCommand != null)
                {
                    telegramCommandList.Add(new BotCommand
                    {
                        Command = helpCommand.Command,
                        Description = helpCommand.Description
                    });
                }

                bot.SetMyCommands(telegramCommandList);

                return bot;
            })
            .AddSingleton<MessageHandler>();

        return services;
    }

    public static IServiceCollection AddTelegramCommandHandlers(this IServiceCollection services)
    {
        var telegramCommandType = typeof(ITelegramCommandHandler);
        var handlers = telegramCommandType.Assembly.GetImplementationTypes<ITelegramCommandHandler>();
        var helpCommandHandlerExists = false;

        foreach (var handler in handlers)
        {
            if (typeof(IHelpCommandHandler).IsAssignableFrom(handler))
            {
                if (helpCommandHandlerExists == false)
                {
                    services.AddSingleton(typeof(IHelpCommandHandler), handler);
                    helpCommandHandlerExists = true;
                }
            }
            else
            {
                services.Add(new ServiceDescriptor(telegramCommandType, handler, ServiceLifetime.Singleton));
            }
        }

        return services;
    }
}