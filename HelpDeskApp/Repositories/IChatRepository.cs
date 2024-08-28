using HelpDeskApp.Models.Domain;

namespace HelpDeskApp.Repositories
{
    public interface IChatRepository
    {
        Task<List<Message>> GetAllMessagesAsync(int id);
        Task<Chat> CreateChatAsync(Chat chat);
        Task<Chat> GetActiveChatByUserId(string userId);
        Task LeaveChatAsync(string userId);
        Task<Chat> GetActiveConsultantChats(string userId, List<Topic> topicList);
        Task<Chat> GetAvailableConsultantChats(string userId);
    }
}
