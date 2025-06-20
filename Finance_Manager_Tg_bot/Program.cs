﻿using Finance_Manager_Tg_bot.BackendApi;
using Finance_Manager_Tg_bot.Models;
using Finance_Manager_Tg_bot.Services;
using Finance_Manager_Tg_bot.Services.AuthServices;
using Finance_Manager_Tg_bot.TelegramApi;
using Finance_Manager_Tg_bot.TelegramApi.Routes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Finance_Manager_Tg_bot;

public class Program
{
    public static IServiceProvider ServiceProvider { get; private set; }
    public static IConfiguration Config { get; private set; }

    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();        

        var botService = ServiceProvider.GetRequiredService<TelegramBotService>();
        await botService.StartAsync();

        // Seed categories
        var apiClient = ServiceProvider.GetRequiredService<ApiClient>();
        CategoriesStorage.AllCategories = await apiClient.GetAllCategoriesAsync();

        Console.ReadLine();
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        // Config
        Config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        services.AddSingleton<IConfiguration>(Config);

        // Logger
        Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog();
        });

        // Cache
        services.AddMemoryCache();

        // Solution without DI:
        /*ILoggerFactory loggerFactory = new SerilogLoggerFactory(Log.Logger);        
        var tgBotLogger = loggerFactory.CreateLogger<TelegramBotService>();*/
        
        services.AddSingleton<UserSessionsManager>();
        services.AddSingleton<UserContext>();

        services.AddHttpClient<ApiClient>();

        // Services
        services.AddSingleton<AuthService>();
        services.AddSingleton<TokensManager>();
        services.AddSingleton<TransactionsService>();
        services.AddSingleton<UsersService>();

        // Routes
        services.AddSingleton<AuthRoute>();        
        services.AddSingleton<StartRoute>();
        services.AddSingleton<TransactionsRoute>();
        services.AddSingleton<UsersRoute>();

        // TelegramAPI
        services.AddSingleton<TelegramBotService>();
        services.AddSingleton<UpdateRouter>();
    }
}
