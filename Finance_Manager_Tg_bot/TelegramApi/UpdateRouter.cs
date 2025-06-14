using Finance_Manager_Tg_bot.BackendApi;
using Finance_Manager_Tg_bot.Models;
using Finance_Manager_Tg_bot.Services.AuthServices;
using Finance_Manager_Tg_bot.Services;
using Finance_Manager_Tg_bot.TelegramApi.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;

namespace Finance_Manager_Tg_bot.TelegramApi;

public class UpdateRouter
{
    private readonly StartRoute _startRoute;    
    private readonly AuthRoute _authRoute;
    private readonly UsersRoute _usersRoute;
    private readonly UserContext _userContext;
    private readonly TransactionsRoute _transactionsRoute;
    private readonly UserSessionsManager _userSessionsManager;
    private readonly AuthService _authService;
    private readonly ILogger<UpdateRouter> _logger;

    private readonly List<IRoute> _routes = new();

    public UpdateRouter(StartRoute startRoute, AuthRoute authRoute, UsersRoute usersRoute,TransactionsRoute transactionsRoute, 
        UserContext userContext, UserSessionsManager userSessionsManager, AuthService authService, ILogger<UpdateRouter> logger)
    {
        _startRoute = startRoute;        
        _authRoute = authRoute;
        _usersRoute = usersRoute;
        _transactionsRoute = transactionsRoute;
        _userContext = userContext;
        _userSessionsManager = userSessionsManager;
        _authService = authService;
        _logger = logger;

        _routes.AddRange(new List<IRoute>
        {
            _startRoute,            
            _authRoute,
            _usersRoute,
            _transactionsRoute
        });
    }

    public async Task RouteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default)
    {
        foreach (var route in _routes)
        {
            if (route.CanHandle(update))
            {
                if (route is UsersRoute || route is TransactionsRoute)
                {
                    var result = await CheckUserSessionInCache(botClient, cancellationToken);
                    if (!result) return;
                }

                await route.HandleUpdateAsync(botClient, update, cancellationToken);
                return;
            }
        }

        bool sessionExist = await CheckUserSessionInCache(botClient, cancellationToken);

        if (!sessionExist) return;
        else
        {
            await botClient.SendMessage(
                chatId: (long)_userContext.TelegramId,
                text: "Unknown message or command.",
                replyMarkup: new InlineKeyboardMarkup(InlineButtons.MenuButtons),
                cancellationToken: cancellationToken);
        }            
    }

    private async Task<bool> CheckUserSessionInCache(ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        long telegramId = (long)_userContext.TelegramId;

        var session = _userSessionsManager.GetUserSession(telegramId);

        if (session == null)
        {
            try
            {
                var newSession = await _authService.AuthenticateUserByTelegramIdAsync(telegramId);

                _userSessionsManager.SetUserSession(telegramId, newSession);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "API error occurred");

                await botClient.SendMessage(
                    chatId: (long)_userContext.TelegramId,
                    text: "Unknown message or command.",
                    replyMarkup: new InlineKeyboardMarkup(InlineButtons.AuthButtons),
                    cancellationToken: cancellationToken);

                return false;
            }
        }

        return true;
    }
}
