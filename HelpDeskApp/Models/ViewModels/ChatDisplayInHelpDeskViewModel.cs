namespace HelpDeskApp.Models.ViewModels
{
    public class ChatDisplayInHelpDeskViewModel
    {
        public int chatId { get; set; }
        public string topicName { get; set; }
        public bool isCurrentConsultantInChat { get; set; } = false;
        public List<string> usernamesInChat { get; set; } = new List<string>();
    }
}
