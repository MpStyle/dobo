using dobo.MessageBuilder;
using dobo.telegram.Command;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace dobo.TelegramCommand;

public class GarbageCommand(GarbageMessageBuilder garbageMessageBuilder) : ITelegramCommandHandler
{
    public string Command { get; } = "garbage";
    public string Description { get; } = "Get information about when to take out the trash";

    public string Handle(string args, Message msg, UpdateType type)
    {
        return garbageMessageBuilder.Build(args);
    }
}