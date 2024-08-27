namespace HelpDeskApp.Models.Domain
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public List<Department> Departments { get; set; }
    }
}
