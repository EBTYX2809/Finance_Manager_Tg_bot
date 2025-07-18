﻿using Finance_Manager_Tg_bot.BackendApi;
using Finance_Manager_Tg_bot.Models;
using Finance_Manager_Tg_bot.Services;
using Finance_Manager_Tg_bot.Services.AuthServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Finance_Manager_Tg_bot.TelegramApi;

public class TelegramBotService
{
	private readonly ITelegramBotClient _botClient;
    private readonly IConfiguration _configuration;
	private readonly UpdateRouter _updateRouter;
    private readonly ILogger<TelegramBotService> _logger;
    private readonly UserContext _userContext;

    public TelegramBotService(UpdateRouter updateRouter, ILogger<TelegramBotService> logger, UserContext userContext, IConfiguration configuration)
    {
        _updateRouter = updateRouter;
        _logger = logger;
        _userContext = userContext;
        _configuration = configuration;
        _botClient = new TelegramBotClient(_configuration["TelegramApiKey"]);
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
	{
		var receiverOptions = new ReceiverOptions { AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery } };

		_botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cancellationToken);

		var me = await _botClient.GetMe();
        Console.WriteLine($"✅ Bot started: @{me.Username}");
    }

    private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
    {
        _userContext.TelegramId = update.Message?.From?.Id ?? update.CallbackQuery?.From.Id ?? 0;       
        await _updateRouter.RouteAsync(client, update, token);
    }

    private Task HandleErrorAsync(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
    {
        _logger.LogError(exception, "Telegram error occurred");

        return Task.CompletedTask;
    }
}
