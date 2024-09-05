using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace HelpDeskAppTests.TestServices
{
    public class TestUserManager : UserManager<IdentityUser>
    {
        public TestUserManager()
            : base(new Mock<IUserStore<IdentityUser>>().Object,
                   new Mock<IOptions<IdentityOptions>>().Object,
                   new Mock<IPasswordHasher<IdentityUser>>().Object,
                   new IUserValidator<IdentityUser>[0],
                   new IPasswordValidator<IdentityUser>[0],
                   new Mock<ILookupNormalizer>().Object,
                   new Mock<IdentityErrorDescriber>().Object,
                   new Mock<IServiceProvider>().Object,
                   new Mock<ILogger<UserManager<IdentityUser>>>().Object)
        {
        }

        public override Task<IdentityUser> FindByEmailAsync(string email)
        {
            if (email == "test@example.com")
            {
                return Task.FromResult(new IdentityUser { Email = email, Id = "1" });
            }
            return Task.FromResult<IdentityUser>(null);
        }

        public override Task<IdentityUser> FindByIdAsync(string userId)
        {
            if (userId == "1")
            {
                return Task.FromResult(new IdentityUser { Email = "test@example.com", Id = "1" });
            }
            return Task.FromResult<IdentityUser>(null);
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync(IdentityUser user)
        {
            return Task.FromResult("validToken");
        }

        public override Task<IdentityResult> ConfirmEmailAsync(IdentityUser user, string token)
        {
            if (token == "validToken")
            {
                return Task.FromResult(IdentityResult.Success);
            }
            return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Invalid token" }));
        }

        public override Task<string> GeneratePasswordResetTokenAsync(IdentityUser user)
        {
            return Task.FromResult("validToken");
        }

        public override Task<IdentityResult> ResetPasswordAsync(IdentityUser user, string token, string newPassword)
        {
            if (token == "validToken" && newPassword == "Test123!")
            {
                return Task.FromResult(IdentityResult.Success);
            }
            return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Invalid token or password" }));
        }
    }
}