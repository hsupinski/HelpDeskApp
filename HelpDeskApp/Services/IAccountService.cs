using HelpDeskApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace HelpDeskApp.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterViewModel model);
        Task<SignInResult> LoginUserAsync(LoginViewModel model);
        Task<string> GetUsernameById(string userId); 
        Task LogoutUserAsync();
        Task<List<IdentityUser>> GetAllAsync();
        Task<List<IdentityUser>> GetUsersInRoleAsync(string roleName);
        Task<List<string>> GetUserRolesAsync(IdentityUser user);
        Task<List<string>> GetAllRolesAsync();
        Task<IdentityUser> GetUserByIdAsync(string userId);
        Task AddUserToRolesAsync(IdentityUser user, List<string> rolesToAdd);
        Task RemoveUserFromRolesAsync(IdentityUser user, List<string> rolesToRemove);
        Task<List<UserRoleViewModel>> GetUserRoleViewModel();
        Task UpdateUserRoles(List<UserRoleViewModel> model);

    }
}
