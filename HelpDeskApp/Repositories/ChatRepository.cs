using HelpDeskApp.Data;
using HelpDeskApp.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApp.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly HelpDeskDbContext _context;
        public ChatRepository(HelpDeskDbContext helpDeskDbContext)
        {
            _context = helpDeskDbContext;
        }

        public async Task<Chat> CreateChatAsync(Chat chat)
        {
            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();
            return chat;
        }

        public async Task<Chat> GetActiveChatByUserId(string userId)
        {
            return await _context.Chats
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.EndTime == null && c.Messages.Any(m => m.SenderId == userId));
        }

        public Task<Chat> GetActiveConsultantChats(string userId, List<Topic> topicList)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Message>> GetAllMessagesAsync(int id)
        {
            return await _context.Messages
                .Where(m => m.ChatId == id)
                .ToListAsync();
        }

        public Task<Chat> GetAvailableConsultantChats(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task LeaveChatAsync(string userId)
        {
            var chat = await GetActiveChatByUserId(userId);
            if (chat != null)
            {
                chat.EndTime = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}
