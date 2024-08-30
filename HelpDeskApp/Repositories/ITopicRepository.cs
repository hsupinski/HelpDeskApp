using HelpDeskApp.Models.Domain;

namespace HelpDeskApp.Repositories
{
    public interface ITopicRepository
    {
        Task<List<Topic>> GetAllAsync();
        Task AddAsync(Topic topic);
        Task<Topic> GetByIdAsync(int id);
        Task UpdateAsync(Topic topic);
        Task DeleteAsync(int id);
        Task DeleteTopicsWithoutDepartment();
        Task<List<Topic>> GetTopicsByDepartmentId(int departmentId);
    }
}
