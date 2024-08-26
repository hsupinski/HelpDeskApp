using HelpDeskApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace HelpDeskApp.Services
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterViewModel model);
        Task<SignInResult> LoginUserAsync(LoginViewModel model);
        Task LogoutUserAsync();
    }
}
