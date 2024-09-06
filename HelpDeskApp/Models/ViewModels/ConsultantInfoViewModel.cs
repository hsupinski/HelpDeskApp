using HelpDeskApp.Models.Domain;

namespace HelpDeskApp.Models.ViewModels
{
    public class ConsultantInfoViewModel
    {
        public List<Topic> TopicList { get; set; } = new List<Topic>();
        public List<Department> DepartmentList { get; set; } = new List<Department>();
        public string Username { get; set; }
    }
}
