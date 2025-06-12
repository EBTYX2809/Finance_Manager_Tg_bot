using Finance_Manager_Tg_bot.BackendApi;
using Finance_Manager_Tg_bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_Tg_bot.Services.AuthServices;

public enum AuthStage
{
    None,
    AwaitingEmail,
    AwaitingPassword
}

public class AuthSession
{
    public AuthStage Stage { get; set; } = AuthStage.None;
    public string? Email { get; set; }
    public bool IsRegisterOrLogin { get; set; } = true;
}

public class AuthService
{
    private readonly ApiClient _client;
    private readonly Dictionary<long, AuthSession> _sessions = new();

    public AuthService(ApiClient client)
    {
        _client = client;
    }

    public AuthSession GetOrCreate(long telegramId)
    {
        if (!_sessions.TryGetValue(telegramId, out AuthSession session)) 
        {
            session = new AuthSession();
            _sessions[telegramId] = session;
        }
        return session;
    }

    public void Clear(long telegramId)
    {
        _sessions.Remove(telegramId);
    }

    public async Task<AuthUserTokensDTO> RegistrationAsync(string email, string password)
    {
        return await _client.RegisterAsync(email, password);
    }

    public async Task<AuthUserTokensDTO> AuthenticateAsync(string email, string password)
    {
        return await _client.AuthenticateAsync(email, password);
    }
}
