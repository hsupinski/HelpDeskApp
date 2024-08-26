namespace HelpDeskApp.Models.Domain
{
    public class Chat
    {
        public int Id { get; set; }
        public string Topic { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsSaved { get; set; } = true; // Default value is true
        public IEnumerable<Message> Messages { get; set; }
        public IEnumerable<ChatParticipation> Participants { get; set; }
    }
}
