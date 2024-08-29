using HelpDeskApp.Data;
using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;
using HelpDeskApp.Repositories;

namespace HelpDeskApp.Services
{
    public class TopicService : ITopicService
    {
        private readonly HelpDeskDbContext _context;
        private readonly ITopicRepository _topicRepository;
        private readonly IDepartmentService _departmentService;
        public TopicService(HelpDeskDbContext helpDeskDbContext, ITopicRepository topicRepository, IDepartmentService departmentService)
        {
            _context = helpDeskDbContext;
            _topicRepository = topicRepository;
            _departmentService = departmentService;
        }

        public async Task AddAsync(Topic topic)
        {
            await _topicRepository.AddAsync(topic);
        }

        public async Task<List<TopicViewModel>> CreateTopicViewModelList(List<Topic> topicList)
        {
            List<TopicViewModel> topicViewModelList = new List<TopicViewModel>();

            foreach (var topic in topicList)
            {
                var departmentNames = new List<string>();

                if (topic.DepartmentIds != null)
                {
                    foreach (var departmentId in topic.DepartmentIds)
                    {
                        var department = await _departmentService.GetByIdAsync(departmentId);
                        if (department != null)
                            departmentNames.Add(department.Name);
                    }
                }

                var topicViewModel = new TopicViewModel
                {
                    Id = topic.Id,
                    Name = topic.Name,
                    DepartmentNames = departmentNames
                };

                topicViewModelList.Add(topicViewModel);
            }

            return topicViewModelList;
        }

        public async Task<TopicViewModel> CreateTopicViewModel(Topic topic)
        {
            var topicViewModel = new TopicViewModel
            {
                Id = topic.Id,
                Name = topic.Name,
                DepartmentNames = new List<string>()
            };

            foreach (var departmentId in topic.DepartmentIds)
            {
                var department = await _departmentService.GetByIdAsync(departmentId);
                topicViewModel.DepartmentNames.Add(department.Name);
            }

            return topicViewModel;
        }

        public async Task DeleteAsync(int id)
        {
            await _topicRepository.DeleteAsync(id);
        }

        public async Task<List<Topic>> GetAllAsync()
        {
            return await _topicRepository.GetAllAsync();
        }

        public async Task<Topic> GetByIdAsync(int id)
        {
            return await _topicRepository.GetByIdAsync(id);
        }

        public Task<List<Topic>> GetTopicsByDepartmentId(int departmentId)
        {
            return _topicRepository.GetTopicsByDepartmentId(departmentId);
        }

        public async Task UpdateAsync(Topic topic)
        {
            _context.Topics.Update(topic);
            await _context.SaveChangesAsync();
        }
    }
}
