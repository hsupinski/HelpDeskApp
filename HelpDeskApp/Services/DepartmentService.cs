using HelpDeskApp.Data;
using HelpDeskApp.Models.Domain;
using HelpDeskApp.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApp.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly HelpDeskDbContext _context;
        private readonly IAccountService _accountService;
        private readonly IDepartmentRepository _departmentRepository;
        public DepartmentService(HelpDeskDbContext helpDeskDbContext, IAccountService accountService, IDepartmentRepository departmentRepository)
        {
            _context = helpDeskDbContext;
            _accountService = accountService;
            _departmentRepository = departmentRepository;
        }

        public async Task AddAsync(Department department, string departmentHeadId)
        {
            department.DepartmentHeadId = departmentHeadId;
            department.ConsultantId = [departmentHeadId];

            // Automatically add all Admins as consultants
            var admins = await _accountService.GetUsersInRoleAsync("Admin");

            foreach (var admin in admins)
            {
                department.ConsultantId.Add(admin.Id);
            }

            await _departmentRepository.AddAsync(department);
        }

        public async Task DeleteAsync(int id)
        {
            await _departmentRepository.DeleteAsync(id);
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await _departmentRepository.GetAllAsync();
        }

        public async Task<Department> GetByIdAsync(int id)
        {
            return await _departmentRepository.GetByIdAsync(id);
        }

        public async Task<List<Department>> GetUserDepartments(string userId)
        {
            return await _departmentRepository.GetUserDepartments(userId);
        }

        public async Task UpdateAsync(Department department, string departmentHeadId)
        {
            department.DepartmentHeadId = departmentHeadId;
            await _departmentRepository.UpdateAsync(department);
        }
    }
}
