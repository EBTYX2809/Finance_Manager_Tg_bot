using Finance_Manager_Tg_bot.TelegramApi.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Finance_Manager_Tg_bot.TelegramApi;

public class UpdateRouter
{
    private readonly StartRoute _startRoute;
    private readonly HelpRoute _helpRoute;
    private readonly AuthRoute _authRoute;

    public UpdateRouter()
    {
        _startRoute = new StartRoute();
        _helpRoute = new HelpRoute();
        _authRoute = new AuthRoute();
    }

    public async Task RouteAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default)
    {
        if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Text)
        {
            var message = update.Message;
            var text = message.Text!.Trim().ToLower();

            switch (text)
            {
                case "/start":
                    await _startRoute.HandleAsync(botClient, message, cancellationToken);
                    break;
                case "/help":
                    await _helpRoute.HandleAsync(botClient, message, cancellationToken);
                    break;

                default:
                    await botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: "Unknown command. Type /help for list of available commands.",
                        cancellationToken: cancellationToken
                    );
                    break;
            }
        }
    }
}
