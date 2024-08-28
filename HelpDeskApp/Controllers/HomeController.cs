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
        private readonly IAccountService _accountService;

        public HomeController(IChatService chatService, ITopicService topicService, IAccountService accountService)
        {
            _chatService = chatService;
            _topicService = topicService;
            _accountService = accountService;
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

            foreach(var username in model.UsersInChatroom)
            {
                Console.WriteLine(username);
            }

            return View(model);
        }

        public async Task<IActionResult> LeaveChat()
        {
            Console.WriteLine("LeaveChat called.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var chat = await _chatService.GetActiveChatByUserId(userId);

            if (chat == null)
            {
                return RedirectToAction("Index");
            }

            // If user leaves the chatroom, close it

            if (userRole == "User")
            {
                await _chatService.LeaveChatAsync(userId);
                await _chatService.KillChatAsync(userId, false);
            }

            else if (userRole == "Consultant")
            {
                var userIds = await _chatService.GetUsersInChat(chat.Id);

                // Consultant is not allowed to leave the user alone,
                // they can only leave if there is another non-admin consultant in the chat

                int count = 0;

                foreach (var _userId in userIds)
                {
                    var user = await _accountService.GetUserByIdAsync(_userId);
                    var roles = await _accountService.GetUserRolesAsync(user);

                    if (!roles.Contains("Admin") && roles.Contains("Consultant"))
                    {
                        count++;
                    }
                }

                if (count > 1)
                {
                    // Leave the chat without killing it
                    await _chatService.LeaveChatAsync(userId);
                    return RedirectToAction("Index");
                }

                else
                {
                    // TODO: Handle error message
                    return RedirectToAction("Chat", new { chatId = chat.Id });
                }
            }

            else if(userRole == "Admin")
            {
                // Leave chat silently
                await _chatService.LeaveChatAsync(userId);
            }

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
