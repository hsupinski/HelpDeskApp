using Microsoft.AspNetCore.Mvc;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        //public IActionResult Index()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    var activeChats = _chatService.GetActiveConsultantChats(userId);
        //    var availableChats = _chatService.GetAvailableConsultantChats(userId);
        //}
    }
}
