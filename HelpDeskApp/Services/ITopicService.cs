using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;

namespace HelpDeskApp.Services
{
    public interface ITopicService
    {
        Task<List<Topic>> GetAllAsync();
        Task AddAsync(Topic topic);
        Task<Topic> GetByIdAsync(int id);
        Task UpdateAsync(Topic topic);
        Task DeleteAsync(int id);
        Task DeleteTopicsWithoutDepartment();
        Task<List<Topic>> GetTopicsByDepartmentId(int departmentId);
        Task<List<TopicViewModel>> CreateTopicViewModelList(List<Topic> topicList);
        Task<TopicViewModel> CreateTopicViewModel(Topic topic);
    }
}
