using dobo.info.weather.MessageBuilder;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace dobo.telegram.Command;

public class WeatherCommand(WeatherMessageBuilder garbageMessageBuilder) : ITelegramCommandHandler
{
    public string Command { get; } = "weather";
    public string Description { get; } = "Get weather information for today";

    public async Task<string?> Handle(string? args, Message msg, UpdateType type)
    {
        var message = await garbageMessageBuilder.Build(args);
        return string.IsNullOrEmpty(message) ? "No weather info" : message;
    }
}