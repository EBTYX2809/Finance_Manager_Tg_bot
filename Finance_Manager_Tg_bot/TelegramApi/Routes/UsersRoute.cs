using Finance_Manager_Tg_bot.BackendApi;
using Finance_Manager_Tg_bot.Models;
using Finance_Manager_Tg_bot.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Finance_Manager_Tg_bot.TelegramApi.Routes;

public class UsersRoute : IRoute
{
    private readonly UsersService _usersService;
    private readonly UserContext _userContext;
    private readonly UserSessionsManager _userSessionsManager;
    private readonly ILogger<UsersRoute> _logger;

    public UsersRoute(UsersService usersService, UserContext userContext, UserSessionsManager userSessionsManager, ILogger<UsersRoute> logger)
    {
        _usersService = usersService;
        _userContext = userContext;
        _userSessionsManager = userSessionsManager;
        _logger = logger;
    }

    public bool CanHandle(Update update)
    {
        return update.CallbackQuery?.Data == "check_balance";
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        long telegramId = (long)_userContext.TelegramId;

        var session = _userSessionsManager.GetUserSession(telegramId);

        try
        {
            var balance = await _usersService.GetBalanceAsync(session.UserDTO.Id);

            await botClient.SendMessage(
                chatId: telegramId,
                text: $"Balance: {balance.PrimaryBalance.Balance} {balance.PrimaryBalance.Currency}.",
                replyMarkup: new InlineKeyboardMarkup(InlineButtons.MenuButtons),
                cancellationToken: token);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(botClient, telegramId, ex, token);
            return;
        }
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, long chatId, Exception exception, CancellationToken token)
    {
        _logger.LogError(exception, "Error in UsersRoute occurred");

        if (exception is ApiException apiException)
        {
            await botClient.SendMessage(
            chatId: chatId,
            text: apiException.Error.Message,
            cancellationToken: token);
        }
        else
        {
            await botClient.SendMessage(
            chatId: chatId,
            text: exception.Message,
            cancellationToken: token);
        }
    }
}
