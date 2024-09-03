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

        public async Task<TopicViewModel> CreateTopicViewModel(Topic topic)
        {
            var model = new TopicViewModel();
            return model;
        }

        public async Task<List<TopicViewModel>> CreateTopicViewModelList(List<Topic> topicList)
        {
            var model = new List<TopicViewModel>();
            return model;
        }

        public async Task DeleteAsync(int id)
        {
            return;
        }

        public async Task DeleteTopicsWithoutDepartment()
        {
            return;
        }

        public async Task<List<Topic>> GetAllAsync()
        {
            var model = new List<Topic>();
            return model;
        }

        public async Task<Topic> GetByIdAsync(int id)
        {
            return new Topic();
        }

        public Task<List<Topic>> GetTopicsByDepartmentId(int departmentId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Topic topic)
        {
            return;
        }
    }
}
