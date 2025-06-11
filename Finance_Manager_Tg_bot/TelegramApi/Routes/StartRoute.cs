using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Finance_Manager_Tg_bot.TelegramApi.Routes;

public class StartRoute : IRoute
{

    public async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken token)
    {
        var welcomeText = """
        👋 Hi! I'm — Telegram-bot for managing your finances.
        I can create new transactions and show your balance. 

        First, you need to log in — click on registration or authenticate button.
        Type /help to see all commands.
        """;

        var buttons = new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Registration", "auth_register"),
                InlineKeyboardButton.WithCallbackData("Authenticate", "auth_login")
            }
        };

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: welcomeText,
            replyMarkup: new InlineKeyboardMarkup(buttons),
            cancellationToken: token
        );
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, long chatId, Exception exception, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
