using dobo.core.Extensions;
using dobo.info.MessageBuilder;
using Microsoft.Extensions.DependencyInjection;

namespace dobo.info.Extension;

public static class IoCInfoExtensions
{
    public static IServiceCollection AddMessageBuilder(this IServiceCollection services)
    {
        var implementationTypes = typeof(IMessageBuilder).Assembly.GetImplementationTypes<IMessageBuilder>();

        foreach (var serviceType in implementationTypes)
        {
            services.AddSingleton(serviceType);
        }

        return services;
    }
}