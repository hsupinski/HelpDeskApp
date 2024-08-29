namespace HelpDeskApp.Models.ViewModels
{
    public class ChatLogsInfoViewModel
    {
        public string TopicName { get; set; }
        public int ChatId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsServiced { get; set; }
    }
}
