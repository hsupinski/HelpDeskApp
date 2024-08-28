using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;

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
        Task<List<Chat>> GetActiveConsultantChats(string userId, List<Topic> topicList);
        Task<List<Chat>> GetAvailableConsultantChats(string userId, List<Topic> topicList);
        Task<List<IdWithUsernameViewModel>> GetUserIdWithoutUsernameInChat(int chatId);
        Task RedirectToDifferentTopic(int chatId, string newTopic);
    }
}
