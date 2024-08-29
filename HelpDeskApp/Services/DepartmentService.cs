using HelpDeskApp.Data;
using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;
using HelpDeskApp.Repositories;
using Microsoft.AspNetCore.Identity;

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

        public async Task AssignConsultants(int departmentId, List<ConsultantInDepartmentViewModel> consultantList)
        {
            var department = await GetByIdAsync(departmentId);

            var consultants = new List<string>();

            foreach (var consultant in consultantList)
            {
                if (consultant.IsInDepartment)
                {
                    consultants.Add(consultant.ConsultantId);
                }
            }

            department.ConsultantId = consultants;
            await UpdateAsync(department, department.DepartmentHeadId);
        }

        public async Task CreateDepartment(Department department, string departmentHeadId)
        {
            var user = await _accountService.GetUserByIdAsync(departmentHeadId);
            var userRoles = await _accountService.GetUserRolesAsync(user);

            // If selected user is not a Department Head, add the role
            if (!userRoles.Contains("Department Head"))
            {
                await _accountService.AddUserToRolesAsync(user, new List<string> { "Department Head" });
            }

            await AddAsync(department, departmentHeadId);
        }

        public async Task<List<DepartmentWithHeadViewModel>> CreateDepartmentWithHeadViewModelList(List<Department> departmentList)
        {
            var model = new List<DepartmentWithHeadViewModel>();

            foreach (var department in departmentList)
            {
                var departmentHead = await _accountService.GetUserByIdAsync(department.DepartmentHeadId);
                model.Add(new DepartmentWithHeadViewModel
                {
                    Id = department.Id,
                    DepartmentHeadName = departmentHead.UserName,
                    DepartmentName = department.Name
                });
            }

            return model;
        }

        public async Task DeleteAsync(int id)
        {
            await _departmentRepository.DeleteAsync(id);
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await _departmentRepository.GetAllAsync();
        }

        public async Task<List<IdentityUser>> GetAllConsultantsAndDepartmentHeads()
        {
            var consultantList = await _accountService.GetUsersInRoleAsync("Consultant");
            var departmentHeadList = await _accountService.GetUsersInRoleAsync("Department Head");

            var users = new List<IdentityUser>();
            users.AddRange(consultantList);
            users.AddRange(departmentHeadList);

            users = users.Distinct().ToList(); // remove duplicates

            return users;
        }

        public async Task<List<ConsultantInDepartmentViewModel>> GetAvailableConsultants(int departmentId)
        {
            var consultants = await _accountService.GetUsersInRoleAsync("Consultant");
            var departmentHeads = await _accountService.GetUsersInRoleAsync("Department Head");
            var department = await GetByIdAsync(departmentId);

            var model = new List<ConsultantInDepartmentViewModel>();

            var availableUsers = new List<IdentityUser>();

            // Both department heads and consultants are available to be assigned to the department

            availableUsers.AddRange(consultants);
            availableUsers.AddRange(departmentHeads);

            // Remove duplicates

            availableUsers = availableUsers.Distinct().ToList();

            // Remove the head of the selected department from the list of available users

            availableUsers.RemoveAll(user => user.Id == department.DepartmentHeadId);

            if (availableUsers != null)
            {
                foreach (var user in availableUsers)
                {
                    var isInDepartment = department.ConsultantId?.Contains(user.Id);

                    model.Add(new ConsultantInDepartmentViewModel
                    {
                        ConsultantId = user.Id,
                        ConsultantName = user.UserName,
                        IsInDepartment = isInDepartment ?? false
                    });
                }
            }

            return model;
        }

        public async Task<Department> GetByIdAsync(int id)
        {
            return await _departmentRepository.GetByIdAsync(id);
        }

        public async Task<List<Department>> GetDepartmentsUserIsAHeadOfAsync(string userId)
        {
            return await _departmentRepository.GetDepartmentsUserIsAHeadOfAsync(userId);
        }

        public async Task<List<Topic>> GetTopicsInDepartment(int departmentId)
        {
            return await _departmentRepository.GetTopicsInDepartment(departmentId);
        }

        public async Task<List<Department>> GetUserDepartments(string userId)
        {
            return await _departmentRepository.GetUserDepartmentsAsync(userId);
        }

        public async Task UpdateAsync(Department department, string departmentHeadId)
        {
            department.DepartmentHeadId = departmentHeadId;
            await _departmentRepository.UpdateAsync(department);
        }
    }
}
