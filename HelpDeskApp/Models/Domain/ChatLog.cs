namespace HelpDeskApp.Models.Domain
{
    public class ChatLog
    {
        public int Id { get; set; }
        public DateTime EventTime { get; set; }
        public string EventType { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public int ChatId { get; set; }
        public string Topic { get; set; }
    }
}
