using dobo.Extension;
using dobo.scheduler.Job;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Telegram.Bot;
using MessageHandler = dobo.telegram.Book.MessageHandler;

var builder = Host.CreateApplicationBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true);
builder.Services.Init();
using var host = builder.Build();

RunBot(host.Services, "DoBo - Lifetime");

await PadovaEstGroupGarbageJob.ScheduleJob(host);
await PadovaEstGroupWeatherJob.ScheduleJob(host);

await host.RunAsync();

return;

static void RunBot(IServiceProvider hostProvider, string lifetime)
{
    using var serviceScope = hostProvider.CreateScope();
    var provider = serviceScope.ServiceProvider;

    var messageHandler = provider.GetService<MessageHandler>();
    var bot = provider.GetService<TelegramBotClient>();

    bot.OnMessage += messageHandler.HandleMessage;
}