using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace Finance_Manager_Tg_bot.TelegramApi.Routes;

public interface IRoute
{
    public Task HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken token);
}
