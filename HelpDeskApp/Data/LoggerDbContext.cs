using HelpDeskApp.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApp.Data
{
    public class LoggerDbContext : DbContext
    {
        public LoggerDbContext(DbContextOptions<LoggerDbContext> options) : base(options)
        {

        }

        public DbSet<ChatLog> ChatLogs { get; set; }
    }
}
