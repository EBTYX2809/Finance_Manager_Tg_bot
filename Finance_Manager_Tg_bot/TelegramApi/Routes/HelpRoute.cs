using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Finance_Manager_Tg_bot.TelegramApi.Routes;

public class HelpRoute : IRoute
{
    public async Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken token)
    {
        var helpText = """
        Available commands:
        /start — Greeting and start of work
        /help — List of available commands
        /add — Create new transaction
        /balance — Show balance
        """;

        await botClient.SendMessage(
            chatId: message.Chat.Id,
            text: helpText,
            cancellationToken: token
        );
    }
}
