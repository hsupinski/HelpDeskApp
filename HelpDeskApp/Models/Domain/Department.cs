namespace HelpDeskApp.Models.Domain
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Chat>? Chats { get; set; }
        public List<string>? ConsultantId { get; set; }
        public string DepartmentHeadId { get; set; }
        public List<Topic>? Topics { get; set; }
    }
}
