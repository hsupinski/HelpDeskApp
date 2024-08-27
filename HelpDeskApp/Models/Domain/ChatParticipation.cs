namespace HelpDeskApp.Models.Domain
{
    public class ChatParticipation
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public string ParticipantId { get; set; }
        public bool IsHidden { get; set; } = false; // Admin can be hidden
    }
}
