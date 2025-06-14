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
    public bool CanHandle(Update update)
    {
        return update.Message?.Text == "/start";
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        var welcomeText = """
        👋 Hi! I'm — Telegram-bot for managing your finances.
        I can create new transactions and show your balance. 

        First, you need to log in — click on registration or authenticate button.        
        """;

        await botClient.SendMessage(
            chatId: update.Message.Chat.Id,
            text: welcomeText,
            replyMarkup: new InlineKeyboardMarkup(InlineButtons.AuthButtons),
            cancellationToken: token
        );
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, long chatId, Exception exception, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
