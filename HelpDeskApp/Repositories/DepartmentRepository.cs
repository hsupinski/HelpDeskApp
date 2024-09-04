using HelpDeskApp.Data;
using HelpDeskApp.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApp.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly HelpDeskDbContext _context;

        public DepartmentRepository(HelpDeskDbContext helpDeskDbContext)
        {
            _context = helpDeskDbContext;
        }
        public async Task AddAsync(Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            _context.Departments.Remove(await _context.Departments.FindAsync(id));

            // Remove all instances of this department from the Topics table

            var topics = await _context.Topics.ToListAsync();
            foreach (var topic in topics)
            {
                topic.DepartmentIds.Remove(id);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await _context.Departments.ToListAsync();
        }

        public async Task<Department> GetByIdAsync(int id)
        {
            return await _context.Departments.FindAsync(id);
        }

        public async Task<List<Department>> GetDepartmentsUserIsAHeadOfAsync(string userId)
        {
            return await _context.Departments
                .Where(d => d.DepartmentHeadId == userId)
                .ToListAsync();
        }

        public async Task<List<Topic>> GetTopicsInDepartment(int departmentId)
        {
            return await _context.Topics
                .Where(t => t.DepartmentIds.Contains(departmentId))
                .ToListAsync();
        }

        public async Task<List<Department>> GetUserDepartmentsAsync(string userId)
        {
            return await _context.Departments
                .Where(d => d.DepartmentHeadId == userId || d.ConsultantId.Contains(userId))
                .ToListAsync();
        }

        public async Task UpdateAsync(Department department)
        {
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
        }
    }
}
