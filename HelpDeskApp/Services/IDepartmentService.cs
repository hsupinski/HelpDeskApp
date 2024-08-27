using HelpDeskApp.Models.Domain;

namespace HelpDeskApp.Services
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetAllAsync();
        Task<Department> GetByIdAsync(int id);
        Task AddAsync(Department department, string departmentHeadId);
        Task UpdateAsync(Department department, string departmentHeadId);
        Task DeleteAsync(int id);
    }
}
