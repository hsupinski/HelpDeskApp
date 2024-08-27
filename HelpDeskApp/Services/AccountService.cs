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
            //return await _userManager.Roles.Select(r => r.Name).ToListAsync();

            List<string> userRoleList = ["User", "Consultant", "Department Head"];

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
    }
}

