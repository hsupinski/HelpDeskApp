using HelpDeskApp.Data;
using HelpDeskApp.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApp.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly LoggerDbContext _context;

        public LogRepository(LoggerDbContext loggerDbContext)
        {
            _context = loggerDbContext;
        }

        public async Task<List<ChatLog>> GetChatLogsByChatId(int chatId)
        {
            return await _context.ChatLogs.Where(c => c.ChatId == chatId).ToListAsync();
        }

        public async Task RemoveUserLogs(string userId, int chatId)
        {
            // Replace every content field with event type = 'MessageSent' with 'Message deleted by user'

            var logs = _context.ChatLogs.Where(c => c.ChatId == chatId && c.UserId == userId && c.EventType == "MessageSent").ToList();
            foreach (var log in logs)
            {
                log.Content = "Message deleted by user";
            }

            await _context.SaveChangesAsync();
        }
    }
}


/*public class ChatLog
{
    public int Id { get; set; }
    public DateTime EventTime { get; set; }
    public string EventType { get; set; }
    public string UserId { get; set; }
    public string Content { get; set; }
    public int ChatId { get; set; }
    public string Topic { get; set; }
}*/