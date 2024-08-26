namespace HelpDeskApp.Models.Domain
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Chat> Chats { get; set; }
        public IEnumerable<ApplicationUser> Consultants { get; set; }
        public IEnumerable<Topic> Topics { get; set; }
    }
}
