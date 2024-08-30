using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
namespace HelpDeskApp.Services
{
    public interface IChatService
    {
        Task<Chat> CreateChatAsync(string userId, int topicId);
        Task<Chat> GetActiveChatByUserId(string userId);
        Task<ChatViewModel> CreateChatViewModel(Chat chat, string userId);
        Task LeaveChatAsync(string userId);
        Task FinishChatAsync(string userId, bool isSaved);
        Task<List<Chat>> GetAvailableConsultantChats(string userId);
        Task<List<Chat>> GetAllOpenChats(string userId);
        Task<List<string>> GetUsersInChat(int chatId);
        Task RedirectToDifferentTopic(int chatId, string newTopic);
        Task JoinChatAsConsultant(int chatId, string userId);
        Task<Chat> GetChatById(int chatId);
        Task<string> GetChatTopic(int chatId);
        Task<List<Chat>> GetAllChatsByTopicName(string topic);
        Task<bool> CheckChatValidity(int chatId, string userId);
    }
}
