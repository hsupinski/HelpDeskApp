namespace HelpDeskApp.Models.Domain
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> DepartmentIds { get; set; }
    }
}
