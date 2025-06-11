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
    private readonly Dictionary<string, AuthSession> _sessions = new();

    public AuthSession GetOrCreate(string userId)
    {
        if (!_sessions.TryGetValue(userId, out AuthSession session)) 
        {
            session = new AuthSession();
            _sessions[userId] = session;
        }
        return session;
    }

    public void Clear(string userId)
    {
        _sessions.Remove(userId);
    }

    public async Task RegistrationAsync(string email, string password)
    {
        // API Call
    }

    public async Task AuthenticateAsync(string email, string password)
    {
        // API Call
    }
}
