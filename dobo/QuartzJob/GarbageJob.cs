using dobo.core.Book;
using dobo.MessageBuilder;
using Microsoft.Extensions.Configuration;
using Quartz;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace dobo.QuartzJob;

public class GarbageJob(TelegramBotClient botClient, GarbageMessageBuilder garbageMessageBuilder, IConfiguration configuration):IJob
{
    private readonly string[] receipts= configuration.GetSection(AppSettingsKey.TelegramReceipts).Get<string[]>() ?? [];
    
    public async Task Execute(IJobExecutionContext context)
    {
        var message = garbageMessageBuilder.Build(string.Empty);
        if (message == null)
        {
            return;
        }
        
        foreach (var receipt in receipts)
        {
            var chatId = new ChatId(receipt);
            await botClient.SendMessage(chatId, message);
        }
    }
}