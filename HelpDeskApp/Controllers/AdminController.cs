using HelpDeskApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HelpDeskApp.Models.ViewModels;

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
            var userList = await _accountService.GetAllAsync();
            var model = new List<UserRoleViewModel>();

            foreach (var user in userList)
            {
                var userRoles = await _accountService.GetUserRolesAsync(user);

                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Roles = userRoles
                };

                model.Add(userRoleViewModel);
            }

            ViewBag.AllRoles = await _accountService.GetAllRolesAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRoleViewModel> model)
        {
            foreach (var userRoleViewModel in model)
            {
                var user = await _accountService.GetUserByIdAsync(userRoleViewModel.UserId);
                var existingRoles = await _accountService.GetUserRolesAsync(user);
                var rolesToAdd = userRoleViewModel.Roles.Except(existingRoles);
                var rolesToRemove = existingRoles.Except(userRoleViewModel.Roles);

                await _accountService.AddUserToRolesAsync(user, rolesToAdd.ToList());
                await _accountService.RemoveUserFromRolesAsync(user, rolesToRemove.ToList());
            }

            return RedirectToAction(nameof(ManageUserRoles));
        }
    }
}
