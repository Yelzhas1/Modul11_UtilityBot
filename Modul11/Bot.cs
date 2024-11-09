using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

public class Bot
{
    private readonly ITelegramBotClient _telegramClient;

    public Bot(string token)
    {
        _telegramClient = new TelegramBotClient(token);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery }
        };

        _telegramClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken: cancellationToken);

        var botInfo = await _telegramClient.GetMeAsync();
        Console.WriteLine($"Bot started: {botInfo.Username}");
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message?.Text != null)
        {
            var message = update.Message;

            if (message.Text == "/start")
            {
                var mainMenu = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton[] { "Подсчитать количество символов", "Вычислить сумму чисел" }
                })
                {
                    ResizeKeyboard = true
                };

                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Выберите действие:",
                    replyMarkup: mainMenu,
                    cancellationToken: cancellationToken);
            }
            else if (message.Text == "Подсчитать количество символов")
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Отправьте сообщение, чтобы узнать количество символов.",
                    cancellationToken: cancellationToken);
            }
            else if (message.Text == "Вычислить сумму чисел")
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Отправьте числа через пробел, чтобы получить их сумму.",
                    cancellationToken: cancellationToken);
            }
            else
            {
                if (message.Text.All(char.IsDigit) || message.Text.Contains(" "))
                {
                    try
                    {
                        var numbers = message.Text.Split(' ').Select(int.Parse);
                        var sum = numbers.Sum();
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: $"Сумма чисел: {sum}",
                            cancellationToken: cancellationToken);
                    }
                    catch
                    {
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Ошибка: пожалуйста, отправьте числа через пробел.",
                            cancellationToken: cancellationToken);
                    }
                }
                else
                {
                    var charCount = message.Text.Length;
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: $"В вашем сообщении {charCount} символов.",
                        cancellationToken: cancellationToken);
                }
            }
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiException => $"Telegram API Error:\n{apiException.ErrorCode}\n{apiException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}