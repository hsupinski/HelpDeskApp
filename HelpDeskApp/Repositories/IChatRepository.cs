using HelpDeskApp.Models.Domain;

namespace HelpDeskApp.Repositories
{
    public interface IChatRepository
    {
        Task<List<Message>> GetAllMessagesAsync(int id);
        Task<Chat> CreateChatAsync(Chat chat);
        Task<Chat> GetChatByIdAsync(int id);
        Task UpdateChatAsync(Chat chat);
        Task<Chat> GetActiveChatByUserId(string userId);
        Task LeaveChatAsync(string userId);
        Task FinishChatAsync(string userId, bool isSaved);
        Task<List<Chat>> GetActiveConsultantChats(string userId, List<Topic> topicList);
        Task<List<Chat>> GetAvailableConsultantChats(string userId, List<Topic> topicList);
        Task<List<string>> GetUserIdsInChat(int chatId);
        Task RedirectToDifferentTopic(int chatId, string newTopic);
        Task<List<Chat>> GetAllOpenChats(string userId);
    }
}
