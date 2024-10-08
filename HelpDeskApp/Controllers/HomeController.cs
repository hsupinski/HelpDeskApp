using HelpDeskApp.Models;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
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
            Log.Information("Index page visited.");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var activeChat = await _chatService.GetActiveChatByUserId(userId);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (activeChat != null)
            {
                return RedirectToAction("Chat", new { chatId = activeChat.Id });
            }

            if (userRole == "User")
            {
                return RedirectToAction("ChooseTopic");
            }

            return View();

        }


        public async Task<IActionResult> ChooseTopic()
        {
            Log.Information("ChooseTopic page visited.");
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole != "User")
            {
                return RedirectToAction("Index");
            }

            var topics = await _topicService.GetAllAsync();

            // Remove all topics that start with "Redirect to:"

            topics = topics.Where(t => !t.Name.StartsWith("Redirect to:")).ToList();

            return View(topics);
        }


        public async Task<IActionResult> CreateChat(int topicId)
        {
            Log.Information("CreateChat called.");
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole != "User")
            {
                return RedirectToAction("Index");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var chat = await _chatService.CreateChatAsync(userId, topicId);
            return RedirectToAction("Chat", new { chatId = chat.Id });
        }

        public async Task<IActionResult> JoinChat(int chatId)
        {
            Log.Information("JoinChat called.");
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == "Consultant" || userRole == "Department Head" || userRole == "Admin")
            {
                await _chatService.JoinChatAsConsultant(chatId, userId);
            }

            return RedirectToAction("Chat", new { chatId });
        }

        public async Task<IActionResult> Chat(int chatId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var chat = await _chatService.GetChatById(chatId);

            if (chat == null)
            {
                return RedirectToAction("Index");
            }

            var model = await _chatService.CreateChatViewModel(chat, userId);

            return View(model);
        }

        public async Task<IActionResult> LeaveChat()
        {
            Log.Information("LeaveChat called.");

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

                TempData["SuccessMessage"] = "Chat closed successfully.";
                Log.Information("User left the chat.");
            }

            else if (userRole == "Consultant")
            {
                // Wait for 500ms to let the user leave the chat
                await Task.Delay(500);

                var userIds = await _chatService.GetUsersInChat(chat.Id);

                if(userIds.Count == 1)
                {
                    // If consultant is the only one in the chat, leave the chat

                    await _chatService.LeaveChatAsync(userId);
                    TempData["SuccessMessage"] = "You left the chat successfully.";
                    Log.Information("Consultant left the chat.");
                    return RedirectToAction("Index");
                }

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

                Console.WriteLine($"Count: {count}");

                if (count > 1)
                {
                    await _chatService.LeaveChatAsync(userId);
                    TempData["SuccessMessage"] = "You left the chat successfully.";
                    Log.Information("Consultant left the chat.");
                    return RedirectToAction("Index");
                }

                else
                {
                    TempData["ErrorMessage"] = "You cannot leave the user alone.";
                    Log.Information("Consultant tried to leave the chat but there is no other consultant.");
                    return RedirectToAction("Chat", new { chatId = chat.Id });
                }
            }

            else if (userRole == "Admin")
            {
                // Leave chat silently
                await _chatService.LeaveChatAsync(userId);
                Log.Information("Admin left the chat.");
                TempData["SuccessMessage"] = "You left the chat successfully.";
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