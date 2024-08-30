namespace HelpDeskApp.Models.ViewModels
{
    public class TopicViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string>? DepartmentNames { get; set; }
    }
}
