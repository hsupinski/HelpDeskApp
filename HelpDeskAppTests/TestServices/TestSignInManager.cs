using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using HelpDeskAppTests.TestServices;
using Microsoft.AspNetCore.Authentication;

public class TestSignInManager : SignInManager<IdentityUser>
{
    public TestSignInManager()
        : base(new TestUserManager(),
               new Mock<IHttpContextAccessor>().Object,
               new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
               new Mock<IOptions<IdentityOptions>>().Object,
               new Mock<ILogger<SignInManager<IdentityUser>>>().Object,
               new Mock<IAuthenticationSchemeProvider>().Object,
               new Mock<IUserConfirmation<IdentityUser>>().Object)
    {
    }

    public override Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
    {
        if (userName == "test" && password == "Test123!")
        {
            return Task.FromResult(SignInResult.Success);
        }
        else if (userName == "test" && password == "Test123!" && lockoutOnFailure)
        {
            return Task.FromResult(SignInResult.LockedOut);
        }
        else
        {
            return Task.FromResult(SignInResult.Failed);
        }
    }

    public override Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberClient)
    {
        if (code == "123456")
        {
            return Task.FromResult(SignInResult.Success);
        }
        else
        {
            return Task.FromResult(SignInResult.Failed);
        }
    }

    public override Task<IdentityUser> GetTwoFactorAuthenticationUserAsync()
    {
        var user = new IdentityUser { UserName = "test", Email = "test@example.com" };
        return Task.FromResult(user);
    }
}