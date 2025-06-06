using Microsoft.Extensions.Configuration;
using Telegram.Bot;

namespace Finance_Manager_Tg_bot;

public class Program
{
    private static string apiKey;

    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        apiKey = config["TelegramApiKey"];

        StartBot().Wait();
    }

    public static async Task StartBot()
    {        
        var bot = new TelegramBotClient(apiKey);
        var me = await bot.GetMe();
        Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
    }
}
