using Finance_Manager_Tg_bot.BackendApi;
using Finance_Manager_Tg_bot.Models;
using Finance_Manager_Tg_bot.Services.AuthServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_Tg_bot.Services;

public enum TransactionCreateStage
{
    None,
    AwaitingName,
    AwaitingPrice,
    AwaitingDate,
    //AwaitingCategoryId
}

public class TransactionCreateSession
{
    public TransactionCreateStage Stage { get; set; } = TransactionCreateStage.None;
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    //public int CategoryId { get; set; }
}

public class TransactionsService
{
    private readonly ApiClient _client;
    private readonly Dictionary<long, TransactionCreateSession> _sessions = new();

    public TransactionsService(ApiClient client)
    {
        _client = client;
    }

    public TransactionCreateSession GetOrCreate(long telegramId)
    {
        if (!_sessions.TryGetValue(telegramId, out TransactionCreateSession session))
        {
            session = new TransactionCreateSession();
            _sessions[telegramId] = session;
        }
        return session;
    }

    public void Clear(long telegramId)
    {
        _sessions.Remove(telegramId);
    }

    public async Task<bool> CreateTransactionAsync(TransactionDTO transactionDTO)
    {
        return await _client.CreateTransactionAsync(transactionDTO);
    }
}
