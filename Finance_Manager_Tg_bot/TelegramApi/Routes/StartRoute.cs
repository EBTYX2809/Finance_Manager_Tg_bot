using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Finance_Manager_Tg_bot.TelegramApi.Routes;

public class StartRoute : IRoute
{
    public async Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken token)
    {
        var welcomeText = """
        👋 Hi! I'm — Telegram-bot for managing your finances.
        I can create new transactions and show your balance. 

        First, you need to log in — click on registration or authenticate button.
        Type /help to see all commands.
        """;        

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: welcomeText,
            cancellationToken: token
        );
    }
}
