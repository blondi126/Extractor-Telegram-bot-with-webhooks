using System.Xml.Serialization;
using CG.Web.MegaApiClient;
using Telegram.Bot;
using Telegram.Bot.Types;
using UnzipBot.Models;

namespace UnzipBot.Services.Actions
{
    public class General
    {
        public static async Task<Message> Usage(ITelegramBotClient bot, Message message)
        {
            return await bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Я принимаю только .zip и .rar файлы.");
        }

        public static async Task<Message> AddMegaAcc(ITelegramBotClient bot, Message message)
        {
            var data = message.Text!.Split(' ');

            var authInfo = new AuthInfo(data[0], data[1]);

            UserInfo.AuthsDictionary.Add(message.Chat.Id, authInfo);

            await Serilyzer.SerializeDictionary();

            return await bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Данные сохранены. Пришлите .zip или .rar файл.");
        }
    }
}
