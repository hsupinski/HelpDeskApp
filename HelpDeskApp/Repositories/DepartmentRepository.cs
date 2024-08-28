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

        public async Task<List<Department>> GetUserDepartments(string userId)
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
