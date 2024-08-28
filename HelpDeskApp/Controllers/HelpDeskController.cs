using Microsoft.AspNetCore.Mvc;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HelpDeskApp.Models.ViewModels;

namespace HelpDeskApp.Controllers
{
    [Authorize(Roles = "Admin, Consultant, Department Head")]
    public class HelpDeskController : Controller
    {
        private readonly IChatService _chatService;

        public HelpDeskController(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<IActionResult> Panel()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var activeChats = await _chatService.GetActiveConsultantChats(userId);
            var availableChats = await _chatService.GetAvailableConsultantChats(userId);

            var model = new List<ChatDisplayInHelpDeskViewModel>();

            foreach(var chat in activeChats)
            {
                model.Add(new ChatDisplayInHelpDeskViewModel
                {
                    chatId = chat.Id,
                    topicName = chat.Topic,
                    isCurrentConsultantInChat = true
                });
            }

            foreach (var chat in availableChats)
            {
                model.Add(new ChatDisplayInHelpDeskViewModel
                {
                    chatId = chat.Id,
                    topicName = chat.Topic,
                });
            }

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
