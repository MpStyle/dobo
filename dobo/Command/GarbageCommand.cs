using dobo.core.Extensions;
using dobo.telegram.Command;
using dobo.waste.collection;
using dobo.waste.collection.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace dobo.Command;

public class GarbageCommand(
    IEnumerable<IGarbageScraper> garbageScrapers,
    IConfiguration configuration,
    ILogger<GarbageCommand> logger) : ITelegramCommandHandler
{
    private readonly Dictionary<string, GarbageDay[]> garbageDaysCache = new();
    private readonly string defaultCity = configuration.GetString("wasteCollection:defaultCity");

    public string Command { get; } = "garbage";
    public string Description { get; } = "Get information about when to take out the trash";

    public string Handle(string args, Message msg, UpdateType type)
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

        if (dayGarbage != null)
        {
            return "Tomorrow" + $": {string.Join(", ", dayGarbage.Types.Select(t=>t.ToString()))}\n";
        }

        return null;
    }
}