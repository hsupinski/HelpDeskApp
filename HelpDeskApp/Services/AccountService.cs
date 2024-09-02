using HelpDeskApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApp.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task AddUserToRolesAsync(IdentityUser user, List<string> rolesToAdd)
        {
            await _userManager.AddToRolesAsync(user, rolesToAdd);
        }

        public async Task<List<IdentityUser>> GetAllAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<List<string>> GetAllRolesAsync()
        {
            List<string> userRoleList = ["User", "Consultant", "Department Head", "Admin"];

            return userRoleList;
        }

        public async Task<IdentityUser> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<string> GetUsernameById(string userId)
        {
            return (await _userManager.FindByIdAsync(userId)).UserName;
        }

        public async Task<List<string>> GetUserRolesAsync(IdentityUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        public async Task<List<UserRoleViewModel>> GetUserRoleViewModel()
        {
            var userList = await GetAllAsync();
            var model = new List<UserRoleViewModel>();

            foreach (var user in userList)
            {
                var userRoles = await GetUserRolesAsync(user);

                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Roles = userRoles
                };

                model.Add(userRoleViewModel);
            }

            return model;
        }

        public async Task<List<IdentityUser>> GetUsersInRoleAsync(string roleName)
        {
            return new List<IdentityUser>(await _userManager.GetUsersInRoleAsync(roleName));
        }

        public async Task<SignInResult> LoginUserAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
        }

        public async Task LogoutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel model)
        {
            var user = new IdentityUser
            {
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            return result;
        }

        public async Task RemoveUserFromRolesAsync(IdentityUser user, List<string> rolesToRemove)
        {
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
        }

        public async Task UpdateUserRoles(List<UserRoleViewModel> model)
        {
            foreach (var userRoleViewModel in model)
            {
                var user = await GetUserByIdAsync(userRoleViewModel.UserId);
                var existingRoles = await GetUserRolesAsync(user);
                var rolesToAdd = userRoleViewModel.Roles.Except(existingRoles);
                var rolesToRemove = existingRoles.Except(userRoleViewModel.Roles);

                await AddUserToRolesAsync(user, rolesToAdd.ToList());
                await RemoveUserFromRolesAsync(user, rolesToRemove.ToList());
            }
        }
    }
}

