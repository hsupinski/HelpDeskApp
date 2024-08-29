namespace HelpDeskApp.Models.ViewModels
{
    public class LogDetailsViewModel
    {
        public DateTime EventTime { get; set; }
        public string EventType { get; set; }
        public string Username { get; set; }
        public string Content { get; set; }
        public string TopicName { get; set; }
    }
}
