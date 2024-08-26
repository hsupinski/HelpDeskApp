using Microsoft.AspNetCore.Identity;

namespace HelpDeskApp.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public IEnumerable<ChatParticipation> Chats { get; set; }
    }

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }
    }
}
