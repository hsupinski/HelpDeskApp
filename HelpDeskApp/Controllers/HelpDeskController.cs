using HelpDeskApp.Models.Domain;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpDeskApp.Controllers
{
    [Authorize(Roles = "Admin, Consultant, Department Head")]
    public class HelpDeskController : Controller
    {
        private readonly IChatService _chatService;
        private readonly IAccountService _accountService;
        private readonly IHelpDeskService _helpDeskService;
        private readonly IDepartmentService _departmentService;

        public HelpDeskController(IChatService chatService, IAccountService accountService, IHelpDeskService helpDeskService, IDepartmentService departmentService)
        {
            _chatService = chatService;
            _accountService = accountService;
            _helpDeskService = helpDeskService;
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Panel()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var availableChats = await _helpDeskService.GetAvailableChats(userId);
            var model = await _helpDeskService.CreateChatDisplayViewModel(availableChats);

            var departments = await _departmentService.GetUserDepartments(userId);
            var topics = new List<Topic>();

            foreach (var department in departments)
            {
                topics.AddRange(await _departmentService.GetTopicsInDepartment(department.Id));
            }

            var topicNames = new List<string>();
            var chatIds = new List<int>();

            foreach (var chat in availableChats)
            {
                chatIds.Add(chat.Id);
            }

            foreach (var topic in topics)
            {
                topicNames.Add(topic.Name);
            }

            ViewBag.Topics = topicNames;
            ViewBag.ChatIds = chatIds;

            return View(model);
        }

        public async Task<IActionResult> JoinChat(int chatId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var chatValidity = await _chatService.CheckChatValidity(chatId, userId);

            if (chatValidity == false)
            {
                TempData["ErrorMessage"] = "Chat is not available";
                return RedirectToAction("Panel");
            }

            await _chatService.JoinChatAsConsultant(chatId, userId);

            return RedirectToAction("Chat", "Home", new { chatId });
        }
    }
}
