using HelpDeskApp.Models;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        public async Task<IActionResult> Chat()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
            var chat = await _chatService.GetActiveChatByUserId(userId);

            if(chat == null)
            {
                chat = await _chatService.CreateChatAsync(userId);
            }

            var model = await _chatService.CreateChatViewModel(chat, userId);

            Console.WriteLine($"Chat ID: {model.ChatId}");

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> LeaveChat()
        {
            Console.WriteLine("LeaveChat called.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var chat = await _chatService.GetActiveChatByUserId(userId);

            if (chat == null)
            {
                return RedirectToAction("Index");
            }

            await _chatService.LeaveChatAsync(userId);

            return RedirectToAction("Index");
            
        }
    }
}
