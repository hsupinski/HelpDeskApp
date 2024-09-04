using HelpDeskApp.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApp.Data
{
    public class HelpDeskDbContext : DbContext
    {
        public HelpDeskDbContext(DbContextOptions<HelpDeskDbContext> options) : base(options)
        {

        }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatParticipation> ChatParticipations { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Topic> Topics { get; set; }
    }
}
