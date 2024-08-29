using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;

namespace HelpDeskApp.Services
{
    public interface IHelpDeskService
    {
        Task<List<ChatDisplayInHelpDeskViewModel>> CreateChatDisplayViewModel(List<Chat> availableChats);
        Task<List<Chat>> GetAvailableChats(string userId);
    }
}
