using Microsoft.AspNetCore.Mvc;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HelpDeskApp.Models.ViewModels;
using HelpDeskApp.Models.Domain;

namespace HelpDeskApp.Controllers
{
    [Authorize(Roles = "Admin, Consultant, Department Head")]
    public class HelpDeskController : Controller
    {
        private readonly IChatService _chatService;
        private readonly IAccountService _accountService;
        private readonly IHelpDeskService _helpDeskService;

        public HelpDeskController(IChatService chatService, IAccountService accountService, IHelpDeskService helpDeskService)
        {
            _chatService = chatService;
            _accountService = accountService;
            _helpDeskService = helpDeskService;
        }

        public async Task<IActionResult> Panel()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var availableChats = await _helpDeskService.GetAvailableChats(userId);
            var model = await _helpDeskService.CreateChatDisplayViewModel(availableChats);

            return View(model);
        }

        public async Task<IActionResult> JoinChat(int chatId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _chatService.JoinChatAsConsultant(chatId, userId);

            return RedirectToAction("Chat", "Home", new { chatId = chatId });
        }
    }
}
