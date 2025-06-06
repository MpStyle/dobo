using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace dobo.telegram.Command;

public interface ITelegramCommandHandler
{
    string Command { get; }
    string Description { get; }
    string? Handle(string? args, Message msg, UpdateType type);
}