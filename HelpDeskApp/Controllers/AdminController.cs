using HelpDeskApp.Models.ViewModels;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAccountService _accountService;

        public AdminController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> ManageUserRoles()
        {
            var model = await _accountService.GetUserRoleViewModel();

            ViewBag.AllRoles = await _accountService.GetAllRolesAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRoleViewModel> model)
        {
            await _accountService.UpdateUserRoles(model);

            return RedirectToAction(nameof(ManageUserRoles));
        }
    }
}
