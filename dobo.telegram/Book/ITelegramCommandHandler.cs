using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace dobo.telegram.Book;

public interface ITelegramCommandHandler
{
    string Command { get; }
    string Description { get; }
    Task<string?> Handle(string? args, Message msg, UpdateType type);
}