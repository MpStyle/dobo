using dobo.core.Book;
using dobo.core.Extensions;
using dobo.core.MessageBuilder;
using dobo.waste.collection.Entities;
using Microsoft.Extensions.Configuration;

namespace dobo.waste.collection.MessageBuilder;

public class GarbageMessageBuilder(
    IEnumerable<IGarbageScraper> garbageScrapers,
    IConfiguration configuration
) : IMessageBuilder
{
    private readonly Dictionary<string, GarbageDay[]> garbageDaysCache = new();
    private readonly string defaultCity = configuration.GetString(AppSettingsKey.WasteCollectionDefaultCity);

    public string? Build(string args)
    {
        var city = args?.Trim().ToLowerInvariant().Split(" ").FirstOrDefault();
        if (string.IsNullOrEmpty(city))
        {
            city = defaultCity;
        }

        var scraper =
            garbageScrapers.FirstOrDefault(s => s.City.Equals(city, StringComparison.InvariantCultureIgnoreCase));
        if (scraper is null)
        {
            return
                $"No garbage scraper found for city: \"{city}\". Available cities: {string.Join(", ", garbageScrapers.Select(s => s.City))}";
        }

        if (!garbageDaysCache.TryGetValue(scraper.City, out var garbageDays))
        {
            garbageDays = scraper.Run().ToArray();
            garbageDaysCache[scraper.City] = garbageDays;
        }

        var (year, month, day) = DateTime.Now.AddDays(1);
        var dayGarbage = garbageDays.FirstOrDefault(g => g.Month == month && g.Day == day && g.Year == year);

        if (dayGarbage != null && dayGarbage.Types.Length != 0)
        {
            return "Tomorrow" + $": {string.Join(", ", dayGarbage.Types.Select(t => t.ToString()))}\n";
        }

        return null;
    }
}