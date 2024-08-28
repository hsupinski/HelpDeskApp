using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;
namespace HelpDeskApp.Services
{
    public interface IChatService
    {
        Task<Chat> CreateChatAsync(string userId, int topicId);
        Task<Chat> GetActiveChatByUserId(string userId);
        Task<ChatViewModel> CreateChatViewModel(Chat chat, string userId);
        Task LeaveChatAsync(string userId);
        Task<Chat> GetAvailableConsultantChats(string userId);
        Task<Chat> GetActiveConsultantChats(string userId);

    }
}
