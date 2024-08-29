using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HelpDeskApp.Services;
using System.Security.Claims;

namespace HelpDeskApp.Controllers
{
    [Authorize(Roles = "Admin, Department Head")]
    public class ChatLogsController : Controller
    {
        private readonly ILogService _logService;
        public ChatLogsController(ILogService logService)
        {
            _logService = logService;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _logService.CreateChatLogsInfoViewModel(userId);

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var model = await _logService.GetChatLogsByChatId(id);

            return View(model);
        }
    }
}
