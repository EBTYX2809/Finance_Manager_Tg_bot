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
    public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken token);
    public Task HandleErrorAsync(ITelegramBotClient botClient, long chatId, Exception exception, CancellationToken token);
    public bool CanHandle(Update update);
}
