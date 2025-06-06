using System.Reflection;
using dobo.core.Extensions;
using dobo.waste.collection.Padova;
using Microsoft.Extensions.DependencyInjection;

namespace dobo.waste.collection.Entities;

public static class WasteCollectionExtensions
{
    public static IServiceCollection AddGarbageScraper(this IServiceCollection services)
    {
        var garbageScraper = Assembly.GetAssembly(typeof(PadovaEstGarbageScraper))
            .GetImplementationTypes<IGarbageScraper>();
        foreach (var scraper in garbageScraper)
        {
            services.AddSingleton(typeof(IGarbageScraper), scraper);
        }

        return services;
    }
}