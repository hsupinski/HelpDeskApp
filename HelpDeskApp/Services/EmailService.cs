using RestSharp;
using RestSharp.Authenticators;
using System.Net.Mail;

namespace HelpDeskApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _apiKey;
        private readonly string _environment;
        private readonly string _smtpServer;
        private readonly int _smtpPort;

        public EmailService(IConfiguration configuration)
        {
            _environment = configuration["ASPNETCORE_ENVIRONMENT"];
            _apiKey = configuration["Mailgun:ApiKey"];
            _smtpServer = configuration["Smtp:Server"];
            _smtpPort = int.Parse(configuration["Smtp:Port"]);
        }
        public async Task SendEmail(string to, string subject, string body)
        {
            if(_environment == "Development")
            {
                // Send email using local SMTP server (Papercut)

                var client = new SmtpClient(_smtpServer, _smtpPort);
                var mailMessage = new MailMessage("HelpDeskApp <no-reply@localhost>", to, subject, body)
                {
                    IsBodyHtml = true
                };

                await client.SendMailAsync(mailMessage);

            }
            else
            {
                // Send email using Mailgun API

                var options = new RestClientOptions("https://api.mailgun.net/v3")
                {
                    Authenticator = new HttpBasicAuthenticator("api", _apiKey)
                };

                var client = new RestClient(options);

                var request = new RestRequest();
                request.AddParameter("domain", "sandboxd39072057744484c875bb3604180bab8.mailgun.org", ParameterType.UrlSegment);
                request.Resource = "{domain}/messages";
                request.AddParameter("from", "HelpDeskApp <mailgun@sandboxd39072057744484c875bb3604180bab8.mailgun.org>");
                request.AddParameter("to", to);
                request.AddParameter("subject", subject);
                request.AddParameter("html", body);
                request.Method = Method.Post;

                client.Execute(request);
            }

            
        }
    }
}
