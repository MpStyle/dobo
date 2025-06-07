using dobo.info.Garbage.MessageBuilder;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace dobo.telegram.Command;

public class GarbageCommand(GarbageMessageBuilder garbageMessageBuilder) : ITelegramCommandHandler
{
    public string Command { get; } = "garbage";
    public string Description { get; } = "Get information about when to take out the trash";

    public async Task<string> Handle(string args, Message msg, UpdateType type)
    {
        var message = await garbageMessageBuilder.Build(args);
        return string.IsNullOrEmpty(message) ? "No garbage for tomorrow" : message;
    }
}