using HelpDeskApp.Models.Domain;

namespace HelpDeskApp.Models.ViewModels
{
    public class ChatViewModel
    {
        public int ChatId { get; set; }
        public string UserId { get; set; } // Id of the user invoking the method
        public string UserName { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}
