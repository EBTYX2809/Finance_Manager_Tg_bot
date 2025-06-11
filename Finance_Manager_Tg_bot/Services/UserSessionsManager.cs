using Finance_Manager_Tg_bot.Models;
using System;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_Tg_bot.Services;

public class UserSessionsManager
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _slidingExpiration = TimeSpan.FromHours(1);

    public UserSessionsManager(IMemoryCache cache)
    {
        _cache = cache;
    }  

    public void SetUserSession(long telegramId, AuthUserTokensDTO session)
    {
        _cache.Set(telegramId, session, new MemoryCacheEntryOptions
        {
            SlidingExpiration = _slidingExpiration
        });
    }

    public AuthUserTokensDTO GetUserSession(long telegramId) 
    {
        if (_cache.TryGetValue(telegramId, out AuthUserTokensDTO tokensDTO)) return tokensDTO;
        else return null;
    }

    public void UpdateUserSession(long telegramId, AuthUserTokensDTO session)
    {
        _cache.Remove(telegramId);
        SetUserSession(telegramId, session);
    }
}