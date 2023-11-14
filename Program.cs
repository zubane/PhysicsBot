using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bots.Http;

internal class Program
{
    static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
            return;
        if (message.Text is not { } messageText)
            return;
        var chatId = message.Chat.Id;

        InlineKeyboardMarkup inlineKeyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(text:"Button",callbackData:"11"),
                InlineKeyboardButton.WithCallbackData(text:"Button",callbackData:"12"),
            },

            new[]
            {
                InlineKeyboardButton.WithCallbackData(text:"Button",callbackData:"21"),
                InlineKeyboardButton.WithCallbackData(text:"Button",callbackData:"22"),
            },
        });

        ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
        {
            new KeyboardButton[] { "Button" },
            new KeyboardButton[] { "Button☎️" },
        })
        {
            ResizeKeyboard = true
        };

        await Console.Out.WriteLineAsync($"Received a '{messageText} message in chat {chatId}");
        
        if(message.Text.ToLower() == "/start")
        {
            await botClient.SendTextMessageAsync
                (
                chatId: chatId,
                text: "Welcome to the physics problem solving help chat.",
                replyMarkup: inlineKeyboard
                );
        }

        else if (update.CallbackQuery is not null)
        {
            var callbackData = update.CallbackQuery.Data;
            switch (callbackData)
            {
                case "12":
                    await botClient.SendTextMessageAsync
                        (
                        chatId: chatId,
                        text: "You're choosed Действие 1"
                        );
                    break;
                case "11":
                    await botClient.SendTextMessageAsync
                        (
                        chatId: chatId,
                        text: "You're choosed Действие 2"
                        );
                    break;
            }
        }

        switch (message.Text.ToLower())
        {
            case "/test":
                await botClient.SendTextMessageAsync
                    (
                    chatId: chatId,
                    text: "test command"
                    );
                break;
        }
    }

    static Task HandlePollingError (ITelegramBotClient botClient, Exception ex, CancellationToken cancellationToken)
    {
        var ErrorMessage = ex switch
        {
            ApiRequestException apiRequestException =>
            $"Telegram API error: \n {apiRequestException.ErrorCode}\n{apiRequestException.Message}",
            _ => ex.ToString()
        };
        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
        
    private static void Main(string[] args)
    {
        var botClient = new TelegramBotClient("6942919846:AAGtjlOrXisi3yF1w0IUlpilLJxGOJsVT7o");
        using CancellationTokenSource cts = new();
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = { }
        };
        botClient.StartReceiving
            (
            updateHandler: HandleUpdateAsync, 
            pollingErrorHandler: HandlePollingError, 
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
            );
        var me = Task.Run(() => botClient.GetMeAsync());
        Console.WriteLine($"Start for listening for @{me.Result}");
        Console.ReadLine();
    }
}