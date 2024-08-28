using HelpDeskApp.Models;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace HelpDeskApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        private readonly IChatService _chatService;
        private readonly ITopicService _topicService;

        public HomeController(IChatService chatService, ITopicService topicService)
        {
            _chatService = chatService;
            _topicService = topicService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var activeChat = await _chatService.GetActiveChatByUserId(userId);

            if (activeChat != null)
            {
                return RedirectToAction("Chat", new { chatId = activeChat.Id });
            }

            return RedirectToAction("ChooseTopic");
        }
        public async Task<IActionResult> ChooseTopic()
        {
            var topics = await _topicService.GetAllAsync();
            return View(topics);
        }

        public async Task<IActionResult> CreateChat(int topicId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var chat = await _chatService.CreateChatAsync(userId, topicId);
            return RedirectToAction("Chat", new { chatId = chat.Id });
        }

        public async Task<IActionResult> JoinChat(int chatId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == "Consultant")
            {
                await _chatService.JoinChatAsConsultant(chatId, userId);
            }

            // Currently only consultants can join chats with normal users

            return RedirectToAction("Chat", new { chatId });
        }

        public async Task<IActionResult> Chat(int chatId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var chat = await _chatService.GetChatById(chatId);

            if (chat == null)
            {
                return RedirectToAction("ChooseTopic");
            }

            var model = await _chatService.CreateChatViewModel(chat, userId);
            return View(model);
        }

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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
