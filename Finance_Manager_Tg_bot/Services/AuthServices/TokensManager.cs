using Finance_Manager_Tg_bot.BackendApi;
using Finance_Manager_Tg_bot.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.Json;

namespace Finance_Manager_Tg_bot.Services.AuthServices;

public class TokensManager
{
    private readonly IServiceProvider _provider; // Need for break cycle references with ApiClient
    private readonly UserSessionsManager _userSessionsManager;
    private readonly UserContext _userContext;

    public TokensManager(IServiceProvider provider, UserSessionsManager userSessionsManager, UserContext userContext)
    {
        _provider = provider;
        _userSessionsManager = userSessionsManager;
        _userContext = userContext;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var telegramId = (long)_userContext.TelegramId;
        var session = _userSessionsManager.GetUserSession(telegramId);

        var accessToken = session.AccessJwtToken;

        if (accessToken != null && IsAccessTokenActual(accessToken))
        {
            return accessToken;
        }
        else
        {
            var refreshToken = session.RefreshToken;
            if (refreshToken == null) return null;

            var _apiClient = _provider.GetRequiredService<ApiClient>();

            var newSession = await _apiClient.RefreshTokenAsync(refreshToken);

            _userSessionsManager.UpdateUserSession(telegramId, newSession);

            return newSession.AccessJwtToken;
        }
    }

    // Copied to tests
    private bool IsAccessTokenActual(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken)) return false;

        try
        {
            var parts = accessToken.Split('.');
            if (parts.Length != 3)
                return false;

            var payload = parts[1];
            // JWT payload is Base64Url encoded
            payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(payload));

            var exp = JsonDocument.Parse(json).RootElement.GetProperty("exp").GetInt64();

            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp);
            return expirationTime > DateTimeOffset.UtcNow;
        }
        catch
        {
            return false;
        }
    }    
}
