using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UnzipBot.Services.Actions;

namespace UnzipBot.Services
{
    public class HandleUpdateService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<HandleUpdateService> _logger;

        public HandleUpdateService(ITelegramBotClient botClient, ILogger<HandleUpdateService> logger)
        {
            _botClient = botClient;
            _logger = logger;
        }

        public async Task ProcessFileAsync(Update update)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(update.Message!),
                _ => UnknownUpdateHandlerAsync(update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(exception);
            }
        }

        private async Task BotOnMessageReceived(Message message)
        {
            _logger.LogInformation("Receive message type: {messageType}", message.Type);

            if (message.Type is not (MessageType.Document or MessageType.Text))
            {
                await General.Usage(_botClient, message);
                return;
            }

            var action = message.Type switch
            {
                MessageType.Text => message.Text switch
                {
                    "/start" => ActionsWithText.StartChat(_botClient, message),
                    "/switch" => ActionsWithText.SwitchMegaAccount(_botClient, message),
                    _ => (message.ReplyToMessage != null && message.ReplyToMessage.Text!.Contains("email")) ? General.AddMegaAcc(_botClient, message) : General.Usage(_botClient, message)
                },

                MessageType.Document => message.Document!.FileName switch
                {
                    { } a when a.Contains(".html") => ActionsWithDocument.ProcessHtmlFile(_botClient, message),
                    { } b when b.Contains(".zip") => ActionsWithDocument.ProcessZipFile(_botClient, message),
                    { } c when c.Contains(".rar") => ActionsWithDocument.ProcessRarFile(_botClient, message),
                    _ => General.Usage(_botClient, message)
                },
                _ => General.Usage(_botClient, message)
            };

            var sentMessage = await action;
            _logger.LogInformation("The message was sent with id: {sentMessageId}", sentMessage.MessageId);


        }

        private Task UnknownUpdateHandlerAsync(Update update)
        {
            _logger.LogInformation("Unknown update type: {updateType}", update.Type);
            return Task.CompletedTask;
        }

        public Task HandleErrorAsync(Exception exception)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogInformation("HandleError: {ErrorMessage}", errorMessage);
            return Task.CompletedTask;
        }
    }
}
