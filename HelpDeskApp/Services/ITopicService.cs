using HelpDeskApp.Models.Domain;

namespace HelpDeskApp.Services
{
    public interface ITopicService
    {
        Task<List<Topic>> GetAllAsync();
        Task AddAsync(Topic topic);
        Task<Topic> GetByIdAsync(int id);
        Task UpdateAsync(Topic topic);
        Task DeleteAsync(int id);
    }
}
