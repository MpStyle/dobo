using dobo.core.Book;
using dobo.core.Extensions;
using dobo.info.Garbage.MessageBuilder;
using Microsoft.Extensions.Configuration;
using Quartz;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace dobo.scheduler.Job;

public class PadovaEstGroupGarbageJob(
    TelegramBotClient botClient,
    GarbageMessageBuilder garbageMessageBuilder,
    IConfiguration configuration) : IJob
{
    private readonly long[]? groups = configuration.GetLongArray(AppSettingsKey.TelegramGroups);

    public async Task Execute(IJobExecutionContext context)
    {
        var message = await garbageMessageBuilder.Build(string.Empty);
        if (message == null)
        {
            return;
        }

        foreach (var group in groups)
        {
            var chatId = new ChatId(group);
            var response = await botClient.SendMessage(chatId, message);
        }
    }
    
    public static async Task ScheduleJob(ISchedulerFactory schedulerFactory)
    {
        var scheduler = await schedulerFactory.GetScheduler();

        const string garbageJobName = nameof(PadovaEstGroupGarbageJob);
        var job = JobBuilder.Create<PadovaEstGroupGarbageJob>()
            .WithIdentity(garbageJobName, $"{garbageJobName}Group")
            .Build();

        // Trigger the job to run now, and then every 40 seconds
        var trigger = TriggerBuilder.Create()
            .WithIdentity($"{garbageJobName}Trigger", $"{garbageJobName}TriggerGroup")
            .StartNow()
            .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(18, 50))
            .Build();

        await scheduler.ScheduleJob(job, trigger);
    }
}