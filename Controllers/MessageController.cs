using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using UnzipBot.Services;

namespace UnzipBot.Controllers
{
    public class MessageController : ControllerBase
    {
        [HttpPost]
        [Route($"bot")]
        public async Task<IActionResult> NewMessage([FromServices] HandleUpdateService handleUpdateService,
            [FromBody] Update update)
        {
            await handleUpdateService.ProcessFileAsync(update);
            return Ok();
        }
    }
}
