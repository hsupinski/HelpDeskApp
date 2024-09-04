using RestSharp;
using RestSharp.Authenticators;

namespace HelpDeskApp.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _apiKey;
        private readonly string _domain;

        public EmailService(IConfiguration configuration)
        {
            _apiKey = configuration["Mailgun:ApiKey"];
        }
        public async Task SendEmail(string to, string subject, string body)
        {
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
