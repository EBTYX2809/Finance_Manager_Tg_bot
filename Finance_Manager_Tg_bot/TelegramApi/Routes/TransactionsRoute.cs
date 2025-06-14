using Finance_Manager_Tg_bot.BackendApi;
using Finance_Manager_Tg_bot.Models;
using Finance_Manager_Tg_bot.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Finance_Manager_Tg_bot.TelegramApi.Routes;

public class TransactionsRoute : IRoute
{
    private readonly TransactionsService _transactionsService;
    private readonly UserContext _userContext;
    private readonly UserSessionsManager _userSessionsManager;
    private readonly ILogger<TransactionsRoute> _logger;

    public TransactionsRoute(TransactionsService transactionsService, UserContext userContext, UserSessionsManager userSessionsManager, ILogger<TransactionsRoute> logger)
    {
        _transactionsService = transactionsService;
        _userContext = userContext;
        _userSessionsManager = userSessionsManager;
        _logger = logger;
    }

    public bool CanHandle(Update update)
    {
        return update.CallbackQuery?.Data == "add_transaction"
            || update.CallbackQuery?.Data?.StartsWith("id:") == true
            || (_transactionsService.GetOrCreateSession((long)_userContext.TelegramId).IsTransactionCreateSessionActive
                && update.Message?.Text != null);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        var telegramId = (long)_userContext.TelegramId;
        var session = _transactionsService.GetOrCreateSession(telegramId);

        if (update.CallbackQuery?.Data == "add_transaction")
        {
            session.Stage = TransactionCreateStage.AwaitingName;
            session.IsTransactionCreateSessionActive = true;

            await botClient.SendMessage(
                chatId: telegramId,
                text: "Enter name of transaction:",
                cancellationToken: token);
            return;
        }

        if (update.Message?.Text is string text || update.CallbackQuery?.Data?.StartsWith("id:") == true)
        {
            text = update.Message?.Text ?? "";
            switch (session.Stage)
            {
                case TransactionCreateStage.AwaitingName:
                    session.Name = text.Trim();
                    session.Stage = TransactionCreateStage.AwaitingPrice;

                    await botClient.SendMessage(
                        chatId: telegramId,
                        text: "Enter price of transaction:",
                        cancellationToken: token);
                    break;

                case TransactionCreateStage.AwaitingPrice:
                    if (!decimal.TryParse(text, out decimal price) || price < 0)
                    {
                        await botClient.SendMessage(
                        chatId: telegramId,
                        text: "Invalid format",
                        cancellationToken: token);
                        return;
                    }

                    session.Price = price;
                    session.Stage = TransactionCreateStage.AwaitingCategoryId;

                    var categoryButtons = new List<InlineKeyboardButton[]>();

                    foreach (var category in CategoriesStorage.AllCategories)
                    {
                        categoryButtons.Add(new[]
                        {
                        InlineKeyboardButton.WithCallbackData(category.Name, $"id:{category.Id}")
                    });
                    }

                    await botClient.SendMessage(
                        chatId: telegramId,
                        text: "Choose transaction's category:",
                        replyMarkup: new InlineKeyboardMarkup(categoryButtons),
                        cancellationToken: token);
                    break;

                case TransactionCreateStage.AwaitingCategoryId:
                    int categoryId = 0;

                    if (update.CallbackQuery?.Data?.StartsWith("id:") == true)
                    {
                        categoryId = int.Parse(update.CallbackQuery.Data.TrimStart('i', 'd', ':'));
                    }
                    else if (update.Message?.Text is string)
                    {
                        await botClient.SendMessage(
                        chatId: telegramId,
                        text: "Don't type anything, just choose category on button:",
                        cancellationToken: token);
                        return;
                    }

                    TransactionDTO transactionDTO = new TransactionDTO
                    {
                        Name = session.Name,
                        Price = session.Price,
                        Date = session.Date,
                        CategoryId = categoryId,
                        UserId = _userSessionsManager.GetUserSession(telegramId).UserDTO.Id
                    };

                    try
                    {
                        await _transactionsService.CreateTransactionAsync(transactionDTO);
                    }
                    catch (Exception ex)
                    {
                        await HandleErrorAsync(botClient, telegramId, ex, token);
                        _transactionsService.Clear(telegramId);
                        return;
                    }

                    await botClient.SendMessage(
                        chatId: telegramId,
                        text: $"Success. Transaction {transactionDTO.Name} added.",
                        replyMarkup: new InlineKeyboardMarkup(InlineButtons.MenuButtons),
                        cancellationToken: token);

                    _transactionsService.Clear(telegramId);
                    break;
            }
        }
        else return;
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, long chatId, Exception exception, CancellationToken token)
    {
        _logger.LogError(exception, "Error in TransactionRoute occurred");

        if (exception is ApiException apiException)
        {
            await botClient.SendMessage(
            chatId: chatId,
            text: apiException.Error.Message,
            cancellationToken: token);
        }
        else
        {
            await botClient.SendMessage(
            chatId: chatId,
            text: exception.Message,
            cancellationToken: token);
        }
    }
}
