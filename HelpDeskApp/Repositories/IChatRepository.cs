using HelpDeskApp.Models.Domain;

namespace HelpDeskApp.Repositories
{
    public interface IChatRepository
    {
        Task<List<Message>> GetAllMessagesAsync(int id);
        Task<Chat> CreateChatAsync(Chat chat);
    }
}
