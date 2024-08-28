using HelpDeskApp.Models.Domain;

namespace HelpDeskApp.Repositories
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAllAsync();
        Task<Department> GetByIdAsync(int id);
        Task AddAsync(Department department);
        Task UpdateAsync(Department department);
        Task DeleteAsync(int id);
        Task<List<Department>> GetUserDepartments(string userId);

    }
}
