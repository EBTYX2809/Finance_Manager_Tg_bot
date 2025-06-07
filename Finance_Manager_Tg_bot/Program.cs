using Finance_Manager_Tg_bot.TelegramApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using Telegram.Bot;

namespace Finance_Manager_Tg_bot;

public class Program
{
    private static string apiKey;

    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        apiKey = config["TelegramApiKey"];

        Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
      
        ILoggerFactory loggerFactory = new SerilogLoggerFactory(Log.Logger);        
        var tgBotLogger = loggerFactory.CreateLogger<TelegramBotService>();

        var updateRouter = new UpdateRouter();
        var botService = new TelegramBotService(apiKey, updateRouter, tgBotLogger);

        await botService.StartAsync();

        Console.ReadLine();
    }
}
