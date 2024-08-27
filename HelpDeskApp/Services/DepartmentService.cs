using HelpDeskApp.Data;
using HelpDeskApp.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApp.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly HelpDeskDbContext _context;
        public DepartmentService(HelpDeskDbContext helpDeskDbContext)
        {
            _context = helpDeskDbContext;
        }

        public async Task AddAsync(Department department, string departmentHeadId)
        {
            department.DepartmentHeadId = departmentHeadId;
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            _context.Departments.Remove(department);
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

        public async Task UpdateAsync(Department department, string departmentHeadId)
        {
            department.DepartmentHeadId = departmentHeadId;
            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
        }
    }
}
