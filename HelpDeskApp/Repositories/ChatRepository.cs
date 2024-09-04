using HelpDeskApp.Data;
using HelpDeskApp.Models.Domain;
using HelpDeskApp.Services;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApp.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly HelpDeskDbContext _context;
        private readonly IAccountService _accountService;
        public ChatRepository(HelpDeskDbContext helpDeskDbContext, IAccountService accountService)
        {
            _context = helpDeskDbContext;
            _accountService = accountService;
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
                    && c.Topic == topic.Name)
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

        public async Task<List<string>> GetUserIdsInChat(int chatId)
        {
            return await _context.ChatParticipations
                .Where(p => p.ChatId == chatId)
                .Select(p => p.ParticipantId)
                .ToListAsync();
        }

        public async Task LeaveChatAsync(string userId)
        {
            var chat = await GetActiveChatByUserId(userId);
            var chatId = chat.Id;

            var chatParticipation = await _context.ChatParticipations
                .FirstOrDefaultAsync(p => p.ChatId == chatId && p.ParticipantId == userId);

            _context.ChatParticipations.Remove(chatParticipation);

            var user = await _accountService.GetUserByIdAsync(userId);
            var userRoles = _accountService.GetUserRolesAsync(user);

            // If user is a default user, close the chat
            if (userRoles.Result.Contains("User"))
            {
                await FinishChatAsync(userId, false);
            }

            await _context.SaveChangesAsync();
        }

        public async Task FinishChatAsync(string userId, bool isSaved) // TODO: handle logging
        {
            var chat = await GetActiveChatByUserId(userId);
            chat.EndTime = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task RedirectToDifferentTopic(int chatId, string topicId)
        {
            // Change the topic of the chat and set IsServiced to false
            var chat = _context.Chats.Find(chatId);

            var topicName = _context.Topics.Find(Int32.Parse(topicId)).Name;

            chat.Topic = topicName;
            chat.IsServiced = false;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateChatAsync(Chat chat)
        {
            _context.Chats.Update(chat);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Chat>> GetAllOpenChats(string userId)
        {
            // Returns every chat that is not closed (to be displayed for admin)

            return await _context.Chats
                .Include(c => c.Messages)
                .Where(c => c.EndTime == null)
                .ToListAsync();
        }

        public async Task<string> GetChatTopic(int chatId)
        {
            var chat = await GetChatByIdAsync(chatId);
            return chat.Topic;
        }

        public async Task<List<Chat>> GetAllChatsByTopicName(string topicName)
        {
            // chat.Topic is a topic name

            return await _context.Chats
                .Include(c => c.Messages)
                .Where(c => c.Topic == topicName)
                .ToListAsync();
        }

        public async Task SetChatSaved(int chatId, bool isSaved)
        {
            var chat = await GetChatByIdAsync(chatId);
            chat.IsSaved = isSaved;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsChatSaved(int chatId)
        {
            var chat = await GetChatByIdAsync(chatId);
            return chat.IsSaved;
        }

        public async Task SetMoreInfo(int chatId, string moreInfo)
        {
            var chat = await GetChatByIdAsync(chatId);
            chat.MoreInfo = moreInfo;

            await _context.SaveChangesAsync();
        }
    }
}
