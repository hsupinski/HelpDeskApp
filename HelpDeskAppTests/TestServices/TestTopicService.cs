using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;
using HelpDeskApp.Services;

namespace HelpDeskAppTests.TestServices
{
    internal class TestTopicService : ITopicService
    {
        public async Task AddAsync(Topic topic)
        {
            return;
        }

        public Task<TopicViewModel> CreateTopicViewModel(Topic topic)
        {
            throw new NotImplementedException();
        }

        public Task<List<TopicViewModel>> CreateTopicViewModelList(List<Topic> topicList)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteTopicsWithoutDepartment()
        {
            return;
        }

        public Task<List<Topic>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Topic> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Topic>> GetTopicsByDepartmentId(int departmentId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Topic topic)
        {
            throw new NotImplementedException();
        }
    }
}
