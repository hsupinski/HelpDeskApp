using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HelpDeskAppTests.TestServices
{
    public static class TestHttpContextFactory
    {
        public static HttpContext CreateHttpContext(string userId, string userRole = null)
        {
            var context = new DefaultHttpContext();
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
            });

            if (userRole != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, userRole));
            }

            context.User = new ClaimsPrincipal(identity);
            return context;
        }
    }
}
