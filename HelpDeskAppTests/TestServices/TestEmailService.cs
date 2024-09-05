using HelpDeskApp.Services;

namespace HelpDeskAppTests.TestServices
{
    public class TestEmailService : IEmailService
    {
        public async Task SendEmail(string to, string subject, string body)
        {
            return;
        }
    }
}
