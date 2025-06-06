using dobo.telegram.Command;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace dobo.TelegramCommand;

public class HelpCommand : IHelpCommandHandler
{
    public string Command => "help";
    public string Description => "Show this help message";
    private readonly List<(string, string)> namesDescriptions = new();

    public HelpCommand(IEnumerable<ITelegramCommandHandler> telegramCommandHandlers)
    {
        namesDescriptions.Add((this.Command, this.Description));
        
        foreach (var telegramCommandHandler in telegramCommandHandlers)
        {
            namesDescriptions.Add((telegramCommandHandler.Command, telegramCommandHandler.Description));
        }
    }

    public string Handle(string args, Message msg, UpdateType type)
    {
        return "Available commands:\n" +
               $"{string.Join("\n", namesDescriptions.Select(nd => $"/{nd.Item1} - {nd.Item2}"))}";
    }
}