namespace HelpDeskApp.Models.Domain
{
    public class Chat
    {
        public int Id { get; set; }
        public string Topic { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsSaved { get; set; } = true; // Default value is true
        public bool IsServiced { get; set; } = false; // Is the chat handled by an agent
        public List<Message> Messages { get; set; }
        public List<ChatParticipation> Participants { get; set; }
    }
}
