using Finance_Manager_Tg_bot.BackendApi;
using Finance_Manager_Tg_bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_Tg_bot.Services;

public class UsersService
{
    private readonly ApiClient _client;

    public UsersService(ApiClient client)
    {
        _client = client;
    }

    public async Task<UserBalanceDTO> GetBalanceAsync(int userId)
    {
        return await _client.GetBalanceAsync(userId);
    }

    public async Task<bool> AddTelegramIdAsync(int userId, long telegramId)
    {
        return await _client.AddTelegramIdToUserAsync(userId, telegramId);
    }
}
