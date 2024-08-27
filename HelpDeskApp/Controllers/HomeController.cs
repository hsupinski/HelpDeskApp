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
        private readonly ITopicService _topicService;

        public HomeController(ILogger<HomeController> logger, IChatService chatService, ITopicService topicService)
        {
            _logger = logger;
            _chatService = chatService;
            _topicService = topicService;
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
        public async Task<IActionResult> Chat(int? topicId = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
            var chat = await _chatService.GetActiveChatByUserId(userId);

            if(chat == null)
            {
                if(!topicId.HasValue)
                {
                    return RedirectToAction("ChooseTopic");
                }

                chat = await _chatService.CreateChatAsync(userId, topicId.Value);
            }

            var model = await _chatService.CreateChatViewModel(chat, userId);

            Console.WriteLine($"Chat ID: {model.ChatId}");

            return View(model);
        }

        public async Task<IActionResult> ChooseTopic()
        {
            var topics = await _topicService.GetAllAsync();
            return View(topics);
        }

        [HttpPost]
        public async Task<IActionResult> ChooseTopic(int topicId)
        {
            return RedirectToAction("Chat", new { topicId });
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
