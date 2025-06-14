using Finance_Manager_Tg_bot.Models;
using Finance_Manager_Tg_bot.Services.AuthServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Finance_Manager_Tg_bot.BackendApi;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiClient> _logger;
    private readonly TokensManager _tokensManager;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<ApiClient> logger, TokensManager tokensManager)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _tokensManager = tokensManager;

        _httpClient.BaseAddress = new Uri(_configuration["BackendUri"]);
    }

    // Helper methods
    public StringContent GetStringContent<T>(T data)
    {
        return new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
    }

    public async Task<T> HandleResponseAsync<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
            return result;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var error = JsonSerializer.Deserialize<ErrorResponse>(errorContent, _jsonOptions);
            _logger.LogError(error.Message, "Error in BackendApi.");
            throw new ApiException(error);
        }
    }

    public async Task<bool> HandleResponseAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var error = JsonSerializer.Deserialize<ErrorResponse>(errorContent, _jsonOptions);
            _logger.LogError(error.Message, "Error in BackendApi.");
            throw new ApiException(error);
        }
    }

    // Auth endpoints
    #region
    public async Task<AuthUserTokensDTO> RegisterAsync(string email, string password)
    {
        AuthDataDTO data = new AuthDataDTO
        {
            email = email,
            password = password
        };

        var content = GetStringContent(data);

        var response = await _httpClient.PostAsync("auth/register", content);

        return await HandleResponseAsync<AuthUserTokensDTO>(response);
    }

    public async Task<AuthUserTokensDTO> AuthenticateAsync(string email, string password)
    {
        AuthDataDTO data = new AuthDataDTO
        {
            email = email,
            password = password
        };

        var content = GetStringContent(data);

        var response = await _httpClient.PostAsync("auth/authenticate", content);   
        
        return await HandleResponseAsync<AuthUserTokensDTO>(response);
    }

    public async Task<AuthUserTokensDTO> RefreshTokenAsync(string refreshToken)
    {
        var content = GetStringContent(refreshToken);

        var response = await _httpClient.PostAsync("auth/refresh-token", content);

        return await HandleResponseAsync<AuthUserTokensDTO>(response);
    }

    public async Task<AuthUserTokensDTO> AuthenticateByTelegramIdAsync(long telegramId)
    {
        var content = GetStringContent(telegramId);

        var response = await _httpClient.PostAsync("auth/telegram-id", content);

        return await HandleResponseAsync<AuthUserTokensDTO>(response);
    }
    #endregion

    // User endpoints
    #region
    public async Task<UserBalanceDTO> GetBalanceAsync(int id)
    {
        var response = await _httpClient.GetAsync($"user/{id}");

        return await HandleResponseAsync<UserBalanceDTO>(response);
    }

    public async Task<bool> AddTelegramIdToUserAsync(int userId, long telegramId)
    {
        UserIdTelegramIdDTO data = new UserIdTelegramIdDTO
        {
            UserId = userId,
            TelegramId = telegramId           
        };

        var content = GetStringContent(data);

        var request = new HttpRequestMessage(HttpMethod.Put, "user/telegramId");
        request.Content = content;

        var accessToken = await _tokensManager.GetAccessTokenAsync();

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);

        return await HandleResponseAsync(response);
    }
    #endregion

    // Transaction endpoints
    #region
    public async Task<bool> CreateTransactionAsync(TransactionDTO transaction)
    {
        var content = GetStringContent(transaction);

        var request = new HttpRequestMessage(HttpMethod.Post, "transactions");
        request.Content = content;

        var accessToken = await _tokensManager.GetAccessTokenAsync();

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);

        return await HandleResponseAsync(response);
    }
    #endregion

    // Category endpoints
    #region
    public async Task<List<CategoryDTO>> GetAllCategoriesAsync()
    {       
        var response = await _httpClient.GetAsync("categories");

        return await HandleResponseAsync<List<CategoryDTO>>(response);
    }
    #endregion
}