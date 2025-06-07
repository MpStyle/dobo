using System.Reflection;
using dobo.core.Extensions;
using dobo.info.Garbage.Padova;
using dobo.waste.collection;
using Microsoft.Extensions.DependencyInjection;

namespace dobo.info.Garbage.Entities;

public static class WasteCollectionExtensions
{
    public static IServiceCollection AddGarbageScraper(this IServiceCollection services)
    {
        var garbageScraper = typeof(PadovaEstGarbageScraper).Assembly
            .GetImplementationTypes<IGarbageScraper>();
        foreach (var scraper in garbageScraper)
        {
            services.AddSingleton(typeof(IGarbageScraper), scraper);
        }

        return services;
    }
}