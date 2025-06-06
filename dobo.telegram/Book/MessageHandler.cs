using System.Text.RegularExpressions;
using dobo.telegram.Command;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace dobo.telegram.Book;

public partial class MessageHandler
{
    [GeneratedRegex(@"^\/([a-zA-Z0-9_]+)")]
    private static partial Regex CommandRegex();

    private readonly TelegramBotClient bot;
    private readonly Dictionary<string, Func<string, Message, UpdateType, string>> commandHandlers = new();
    private readonly ILogger<MessageHandler> logger;

    public MessageHandler(TelegramBotClient bot, IEnumerable<ITelegramCommandHandler> telegramCommandHandlers, IHelpCommandHandler helpCommand, ILogger<MessageHandler> logger)
    {
        this.bot = bot;
        this.logger = logger;

        foreach (var telegramCommandHandler in telegramCommandHandlers)
        {
            commandHandlers.Add(telegramCommandHandler.Command, telegramCommandHandler.Handle);
        }
        
        commandHandlers.Add(helpCommand.Command, helpCommand.Handle);
    }

    public async Task HandleMessage(Message msg, UpdateType type)
    {
        if (msg.Text is null)
        {
            return;
        }

        Console.WriteLine($"Received {type} '{msg.Text}' in {msg.Chat}");

        var commandMatch = CommandRegex().Match(msg.Text);

        if (commandMatch.Success)
        {
            var command = commandMatch.Groups[1].Value;
            if (commandHandlers.TryGetValue(command, out var handler))
            {
                var args = msg.Text.Replace($"/{command}", string.Empty).Trim();
                var responseText = handler(args, msg, type);

                if (responseText != null)
                {
                    await bot.SendMessage(msg.Chat, responseText);

                    this.logger.LogInformation("Sent {Type} \'{ResponseText}\' in {MsgChat}", type, responseText, msg.Chat);
                }
                else
                {
                    this.logger.LogInformation("No message to send for command \'{Command}\' in {MsgChat}", command, msg.Chat);
                }
            }
        }
    }
}