using AngleSharp.Html.Parser;
using Aspose.Zip.Rar;
using CG.Web.MegaApiClient;
using System.IO.Compression;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using UnzipBot.Models;
using INode = CG.Web.MegaApiClient.INode;
using NodeType = CG.Web.MegaApiClient.NodeType;

namespace UnzipBot.Services.Actions
{
    public class ActionsWithDocument
    {
        public static async Task<Message> ProcessHtmlFile(ITelegramBotClient bot, Message message)
        {
            var parser = new HtmlParser();

            var fileName =  DownloadFileFromTg(bot, message).Result;
           
            var document = parser.ParseDocument(fileName);
       
            var client = new HttpClient();
            var folder = fileName.Split(".html")[0];

            var images = document.QuerySelectorAll("a");

            foreach (var element in images)
            {
                var image = element.GetAttribute("href");
                DownloadImage(client, image, folder);
            }

            var folderPath = Path.Combine("wwwroot", folder);
            var link = await UploadToMega(folderPath,message.Chat.Id);
            
            return await bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Обработан .html файл:\n{link}");
        }

        public static async Task<Message> ProcessZipFile(ITelegramBotClient bot, Message message)
        {


            var filePath = DownloadFileFromTg(bot, message).Result;
            var newFolderPath = filePath.Split(".zip")[0];

            ZipFile.ExtractToDirectory(filePath, newFolderPath, Encoding.UTF8);


            var link = await UploadToMega(newFolderPath,message.Chat.Id);

            Directory.Delete(newFolderPath);

            return await bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Обработан .zip файл.\n{link}");
        }

        public static async Task<Message> ProcessRarFile(ITelegramBotClient bot, Message message)
        {
            var filePath = DownloadFileFromTg(bot, message).Result;
            var newFolderPath = filePath.Split(".rar")[0];

            using (var archive = new RarArchive(filePath))
                archive.ExtractToDirectory(newFolderPath);

            var link = await UploadToMega(newFolderPath,message.Chat.Id);

            return await bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Обработан .rar файл.\n{link}");
        }


        public static async Task<string> DownloadFileFromTg(ITelegramBotClient bot, Message message)
        {
            var file = await bot.GetFileAsync(message.Document!.FileId);
            var path = Path.Combine("wwwroot", message.Document.FileName!);

            await using var fs = new FileStream(path, FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath!, fs);

            return path;
        }

        private static async void DownloadImage(HttpClient client, string? url, string folder)
        {
            if (url == null)
                return;

            var link = new Uri(url);
            var fileName = url.Split('?')[0].Split('/').Last()!;
            var path = Path.Combine("wwwroot", folder, fileName);

            var response = await client.GetAsync(link);
            await using var fs = new FileStream(path, FileMode.Create);
            await response.Content.CopyToAsync(fs);
        }



        public static async Task<Uri> UploadToMega(string folderPath,long chatId)
        {
            var client = new MegaApiClient();

            var data = UserInfo.AuthsDictionary[chatId];
            var loginInfo = client.GenerateAuthInfos(data.Email, data.Password);

            await client.LoginAsync(loginInfo);

            IEnumerable<INode> nodes = await client.GetNodesAsync();

            var root = nodes.Single(x => x.Type == NodeType.Root);
            var myFolder = await client.CreateFolderAsync(folderPath.Split('\\').Last(), root);

            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith("jpg")
                            || s.EndsWith(".jpeg")
                            || s.EndsWith(".png")
                            || s.EndsWith(".mp4"));

            foreach (var file in files)
                await client.UploadFileAsync(file, myFolder);

            var downloadLink = await client.GetDownloadLinkAsync(myFolder);

            await client.LogoutAsync();

            return downloadLink;
        }
    }
}
