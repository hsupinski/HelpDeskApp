using HelpDeskApp.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace HelpDeskApp.Models.ViewModels
{
    public class DepartmentViewModel
    {
        public List<Department> Departments { get; set; } = new List<Department>();
        public List<Topic> Topics { get; set; } = new List<Topic>();
        public List<IdentityUser> UserList { get; set; } = new List<IdentityUser>();
    }
}
