using HelpDeskApp.Data;
using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;
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
            // Get all chats that contain the user in the participant list and have not ended

            return await _context.Chats
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.EndTime == null && c.Participants.Any(p => p.ParticipantId == userId));
        }

        public async Task<List<Chat>> GetActiveConsultantChats(string userId, List<Topic> topicList)
        {
            /*
            * Active consultant chat:
            * - Is not closed (EndTime is null)
            * - IsServiced does not have to be true, it can be false if it's redirected to a different consultant
            * - Participant list contains the consultant
            * - Chat topic belongs to the consultant's department
            */

            var chatList = new List<Chat>();

            foreach (var topic in topicList) 
            {
                var chats = _context.Chats
                    .Include(c => c.Messages)
                    .Where(c => c.EndTime == null 
                    && c.Participants.Any(p => p.ParticipantId == userId) 
                    && c.Topic.StartsWith(topic.Name))
                    .ToList();
                chatList.AddRange(chats);
            }

            return chatList;
        }

        public async Task<List<Message>> GetAllMessagesAsync(int id)
        {
            return await _context.Messages
                .Where(m => m.ChatId == id)
                .ToListAsync();
        }

        public async Task<List<Chat>> GetAvailableConsultantChats(string userId, List<Topic> topicList)
        {
            /*
             * Available consultant chat:
             * - Is not closed (EndTime is null)
             * - IsServiced is false
             * - Participant list does not contain the consultant
             * - Chat topic belongs to the consultant's department
             */

            var chatList = new List<Chat>();

            foreach (var topic in topicList)
            {
                var chats = _context.Chats
                    .Include(c => c.Messages)
                    .Where(c => c.EndTime == null
                    && !c.IsServiced
                    && !c.Participants.Any(p => p.ParticipantId == userId)
                    && c.Topic.StartsWith(topic.Name))
                    .ToList();
                chatList.AddRange(chats);
            }

            return chatList;
        }

        public async Task<Chat> GetChatByIdAsync(int id)
        {
            return await _context.Chats
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<IdWithUsernameViewModel>> GetUserIdWithoutUsernameInChat(int chatId)
        {
            return await _context.ChatParticipations
                .Where(p => p.ChatId == chatId)
                .Select(p => new IdWithUsernameViewModel
                {
                    UserId = p.ParticipantId,
                })
                .ToListAsync();
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

        public Task RedirectToDifferentTopic(int chatId, string newTopic)
        {
            // Change the topic of the chat and set IsServiced to false
            var chat = _context.Chats.Find(chatId);

            chat.Topic = newTopic;
            chat.IsServiced = false;

            return _context.SaveChangesAsync();
        }

        public async Task UpdateChatAsync(Chat chat)
        {
            _context.Chats.Update(chat);
            await _context.SaveChangesAsync();
        }
    }
}
