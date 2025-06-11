using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Finance_Manager_Tg_bot.TelegramApi.Routes;

public class UsersRoute : IRoute
{
    public async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken token)
    {
        
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, long chatId, Exception exception, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}
