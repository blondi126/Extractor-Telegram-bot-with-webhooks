using Microsoft.AspNetCore.Mvc;

namespace UnzipBot.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Ok("Bot is working");
        }
    }
}