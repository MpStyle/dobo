using System.Reflection;
using dobo.Command;
using dobo.telegram.Extension;
using Microsoft.Extensions.DependencyInjection;

namespace dobo.Extension;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Init(this IServiceCollection services)
    {
        services
            .AddTelegramCommandHandler(Assembly.GetAssembly(typeof(HelpCommand)))
            .AddTelegramBotClient();

        return services;
    }
}