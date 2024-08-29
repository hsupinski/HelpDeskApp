using HelpDeskApp.Models.Domain;

namespace HelpDeskApp.Repositories
{
    public interface ILogRepository
    {
        Task<List<ChatLog>> GetChatLogsByChatId(int chatId);
    }
}
