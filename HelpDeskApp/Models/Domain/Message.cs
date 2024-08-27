namespace HelpDeskApp.Models.Domain
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime TimeSent { get; set; }
        public string SenderId { get; set; }
        public string SenderUsername { get; set; }
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}
