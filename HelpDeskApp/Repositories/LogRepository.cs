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
    }
}
