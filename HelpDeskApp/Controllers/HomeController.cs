using HelpDeskApp.Models;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace HelpDeskApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IChatService _chatService;

        public HomeController(ILogger<HomeController> logger, IChatService chatService)
        {
            _logger = logger;
            _chatService = chatService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Chat()
        {
            if(!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }

            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var model = await _chatService.CreateChatAsync(userId);

                return View(model);
            }
        }
    }
}
