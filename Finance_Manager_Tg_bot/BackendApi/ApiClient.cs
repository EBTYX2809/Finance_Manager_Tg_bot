using Finance_Manager_Tg_bot.Models;
using Finance_Manager_Tg_bot.Services.AuthServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Finance_Manager_Tg_bot.BackendApi;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiClient> _logger;
    private readonly TokensManager _tokensManager;

    public ApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<ApiClient> logger, TokensManager tokensManager)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _tokensManager = tokensManager;

        _httpClient.BaseAddress = new Uri(_configuration["BackendUri"]);
    }

    public async Task<AuthUserTokensDTO> AuthenticateAsync(string email, string password)
    {
        AuthDataDTO data = new AuthDataDTO
        {
            email = email,
            password = password
        };
        
        var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("auth/authenticate", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthUserTokensDTO>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            return result;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var error = JsonSerializer.Deserialize<ErrorResponse>(errorContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            throw new ApiException(error);
        }
    }

    public async Task<AuthUserTokensDTO> RefreshTokenAsync(string refreshToken)
    {
        return null;
    }
}