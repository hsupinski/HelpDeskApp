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

        public async Task<List<Message>> GetAllMessagesAsync(int id)
        {
            return await _context.Messages
                .Where(m => m.ChatId == id)
                .ToListAsync();
        }
    }
}
