using HelpDeskApp.Data;
using HelpDeskApp.Models.Domain;
using HelpDeskApp.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApp.Services
{
    public class TopicService : ITopicService
    {
        private readonly HelpDeskDbContext _context;
        private readonly ITopicRepository _topicRepository;
        public TopicService(HelpDeskDbContext helpDeskDbContext, ITopicRepository topicRepository)
        {
            _context = helpDeskDbContext;
            _topicRepository = topicRepository;
        }

        public async Task AddAsync(Topic topic)
        {
            await _topicRepository.AddAsync(topic);
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
