namespace HelpDeskApp.Models.ViewModels
{
    public class JoinChatItemViewModel
    {
        public int chatId { get; set; }
        public string topicName { get; set; }
        public List<string> usernamesInChat { get; set; } = new List<string>();
    }
}
