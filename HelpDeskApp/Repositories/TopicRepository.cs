using HelpDeskApp.Data;
using HelpDeskApp.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApp.Repositories
{
    public class TopicRepository : ITopicRepository
    {
        private readonly HelpDeskDbContext _context;
        public TopicRepository(HelpDeskDbContext helpDeskDbContext)
        {
            _context = helpDeskDbContext;
        }
        
        public async Task AddAsync(Topic topic)
        {
            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            _context.Topics.Remove(await _context.Topics.FindAsync(id));
            await _context.SaveChangesAsync();
        }

        public async Task<List<Topic>> GetAllAsync()
        {
            _context.Topics.ToList();
            return await _context.Topics.ToListAsync();
        }

        public async Task<Topic> GetByIdAsync(int id)
        {
            return await _context.Topics.FindAsync(id);
        }

        public async Task<List<Topic>> GetTopicsByDepartmentId(int departmentId)
        {
            return await _context.Topics.Where(t => t.DepartmentIds.Contains(departmentId)).ToListAsync();
        }

        public async Task UpdateAsync(Topic topic)
        {
            _context.Topics.Update(topic);
            await _context.SaveChangesAsync();
        }
    }
}
