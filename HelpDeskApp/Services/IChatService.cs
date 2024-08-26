using HelpDeskApp.Models.ViewModels;

namespace HelpDeskApp.Services
{
    public interface IChatService
    {
        Task<ChatViewModel> CreateChatAsync(string userId);
    }
}
