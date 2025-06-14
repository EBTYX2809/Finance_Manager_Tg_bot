using Finance_Manager_Tg_bot.BackendApi;
using Finance_Manager_Tg_bot.Models;
using Finance_Manager_Tg_bot.Services;
using Finance_Manager_Tg_bot.Services.AuthServices;
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

public class AuthRoute : IRoute
{
    private readonly AuthService _authService;
    private readonly UserContext _userContext;
    private readonly UsersService _usersService; 
    private readonly UserSessionsManager _userSessionsManager;
    private readonly ILogger<AuthRoute> _logger;

    public AuthRoute(AuthService authService, UserContext userContext, UsersService usersService, UserSessionsManager userSessionsManager, ILogger<AuthRoute> logger)
    {
        _authService = authService;
        _userContext = userContext;
        _usersService = usersService;
        _userSessionsManager = userSessionsManager;
        _logger = logger;
    }

    public bool CanHandle(Update update)
    {
        return update.CallbackQuery?.Data?.StartsWith("auth_") == true
            || (_authService.GetOrCreateSession((long)_userContext.TelegramId).IsAuthSessionActive
                && update.Message?.Text != null);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        var telegramId = (long)_userContext.TelegramId;
        var session = _authService.GetOrCreateSession(telegramId);

        if (update.CallbackQuery?.Data == "auth_login" || update.CallbackQuery?.Data == "auth_register")
        {
            session.IsRegisterOrLogin = update.CallbackQuery.Data == "auth_register";
            session.Stage = AuthStage.AwaitingEmail;
            session.IsAuthSessionActive = true;

            await botClient.SendMessage(
                chatId: telegramId,
                text: "Enter email:",
                cancellationToken: token);
            return;
        }        

        if (update.Message?.Text is not string text) return;

        switch (session.Stage) 
        {
            case AuthStage.AwaitingEmail:
                session.Email = text.Trim();
                session.Stage = AuthStage.AwaitingPassword;

                await botClient.SendMessage(
                    chatId: telegramId,
                    text: "Enter password:",
                    cancellationToken: token);
                break;

            case AuthStage.AwaitingPassword:
                var password = text.Trim();

                AuthUserTokensDTO newSession = new();
                try
                {
                    newSession = session.IsRegisterOrLogin == true
                    ? await _authService.RegistrationAsync(session.Email, password)
                    : await _authService.AuthenticateAsync(session.Email, password);

                    // Later operations
                    _userSessionsManager.SetUserSession(telegramId, newSession);

                    await _usersService.AddTelegramIdAsync(newSession.UserDTO.Id, telegramId);
                }
                catch (Exception ex)
                {
                    await HandleErrorAsync(botClient, telegramId, ex, token);
                    _authService.Clear(telegramId);
                    return;
                }                

                await botClient.SendMessage(
                    chatId: telegramId,
                    text: $"Success. Welcome {newSession.UserDTO.Email}",
                    replyMarkup: new InlineKeyboardMarkup(InlineButtons.MenuButtons),
                    cancellationToken: token);

                _authService.Clear(telegramId);
                break;
        }
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, long chatId, Exception exception, CancellationToken token)
    {
        _logger.LogError(exception, "Error in AuthRoute occurred");

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
