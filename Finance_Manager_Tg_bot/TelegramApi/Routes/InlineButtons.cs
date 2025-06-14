using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace Finance_Manager_Tg_bot.TelegramApi.Routes;

public static class InlineButtons
{
    public static InlineKeyboardButton[][] MenuButtons = new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Add transaction", "add_transaction"),
            InlineKeyboardButton.WithCallbackData("Check balance", "check_balance")
        }
    };
    
    public static InlineKeyboardButton[][] AuthButtons = new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Registration", "auth_register"),
            InlineKeyboardButton.WithCallbackData("Authenticate", "auth_login")
        }
    };
}
