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
        Task<List<Department>> GetUserDepartmentsAsync(string userId);
        Task<List<Department>> GetDepartmentsUserIsAHeadOfAsync(string userId);
        Task<List<Topic>> GetTopicsInDepartment(int departmentId);
    }
}
