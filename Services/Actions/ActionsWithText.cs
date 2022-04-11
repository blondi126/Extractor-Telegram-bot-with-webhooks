using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using UnzipBot.Models;

namespace UnzipBot.Services.Actions
{
    public class ActionsWithText
    {
        public static async Task<Message> StartChat(ITelegramBotClient bot, Message message)
        {
            return await bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Добро пожаловать! Введите email и password от аккаунта на mega.nz одним сообщением через пробел.",
                replyMarkup: new ForceReplyMarkup()
                );
        }

        public static async Task<Message> SwitchMegaAccount(ITelegramBotClient bot, Message message)
        {
            UserInfo.AuthsDictionary.Remove(message.Chat.Id);
            await Serilyzer.SerializeDictionary();

            return await bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Введите email и password от нового аккаунта на mega.nz одним сообщением через пробел.",
                replyMarkup: new ForceReplyMarkup()
            );
        }
    }
}
