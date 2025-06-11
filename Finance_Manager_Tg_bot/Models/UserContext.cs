using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_Tg_bot.Models;

public class UserContext
{
    private static readonly AsyncLocal<long?> _currentTelegramId = new();

    public long? TelegramId
    {
        get => _currentTelegramId.Value;
        set => _currentTelegramId.Value = value;
    }
}
