using HelpDeskApp.Models.ViewModels;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Identity;

namespace HelpDeskAppTests.TestServices
{
    public class TestAccountService : IAccountService
    {
        public Task AddUserToRolesAsync(IdentityUser user, List<string> rolesToAdd)
        {
            return Task.CompletedTask;
        }

        public Task<List<IdentityUser>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> GetAllRolesAsync()
        {
            return new List<string> { "User", "Consultant", "Department Head", "Admin" };
        }

        public Task<IdentityUser> GetUserByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUsernameById(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetUserRolesAsync(IdentityUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserRoleViewModel>> GetUserRoleViewModel()
        {
            return new List<UserRoleViewModel>
            {
                new UserRoleViewModel
                {
                    UserId = "1",
                    UserName = "test",
                    Roles = new List<string> { "User" }
                }
            };
        }

        public Task<List<IdentityUser>> GetUsersInRoleAsync(string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<SignInResult> LoginUserAsync(LoginViewModel model)
        {
            if (model.Username == "invalid")
            {
                return Task.FromResult(SignInResult.Failed);
            }

            return Task.FromResult(SignInResult.Success);
        }

        public Task LogoutUserAsync()
        {
            return Task.CompletedTask;
        }

        public Task<IdentityResult> RegisterUserAsync(RegisterViewModel model)
        {
            if (model.Username == "invalid")
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Failed to register user" }));
            }

            return Task.FromResult(IdentityResult.Success);
        }

        public Task RemoveUserFromRolesAsync(IdentityUser user, List<string> rolesToRemove)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateUserRoles(List<UserRoleViewModel> model)
        {
            return;
        }
    }
}
