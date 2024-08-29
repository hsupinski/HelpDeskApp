using HelpDeskApp.Models.ViewModels;

namespace HelpDeskApp.Services
{
    public interface ILogService
    {
        Task<List<ChatLogsInfoViewModel>> CreateChatLogsInfoViewModel(string userId);
        Task<List<LogDetailsViewModel>> GetChatLogsByChatId(int chatId);
    }
}
