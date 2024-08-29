using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace HelpDeskApp.Services
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetAllAsync();
        Task<Department> GetByIdAsync(int id);
        Task AddAsync(Department department, string departmentHeadId);
        Task UpdateAsync(Department department, string departmentHeadId);
        Task DeleteAsync(int id);
        Task CreateDepartment(Department department, string departmentHeadId);
        Task<List<Department>> GetUserDepartments(string userId);
        Task<List<DepartmentWithHeadViewModel>> CreateDepartmentWithHeadViewModelList(List<Department> departmentList);
        Task<List<IdentityUser>> GetAllConsultantsAndDepartmentHeads();
        Task<List<ConsultantInDepartmentViewModel>> GetAvailableConsultants(int departmentId);
        Task AssignConsultants(int departmentId, List<ConsultantInDepartmentViewModel> consultantList);
    }
}
