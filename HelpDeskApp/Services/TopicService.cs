using HelpDeskApp.Data;
using HelpDeskApp.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApp.Services
{
    public class TopicService : ITopicService
    {
        private readonly HelpDeskDbContext _context;
        public TopicService(HelpDeskDbContext helpDeskDbContext)
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
            var topic = await _context.Topics.FindAsync(id);
            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Topic>> GetAllAsync()
        {
            return await _context.Topics.ToListAsync();
        }

        public async Task<Topic> GetByIdAsync(int id)
        {
            return await _context.Topics.FindAsync(id);
        }

        public async Task UpdateAsync(Topic topic)
        {
            _context.Topics.Update(topic);
            await _context.SaveChangesAsync();
        }
    }
}
