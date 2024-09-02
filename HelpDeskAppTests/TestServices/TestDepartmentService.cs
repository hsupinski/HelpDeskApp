using HelpDeskApp.Models.Domain;
using HelpDeskApp.Models.ViewModels;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Identity;

namespace HelpDeskAppTests.TestServices
{
    internal class TestDepartmentService : IDepartmentService
    {
        public Task AddAsync(Department department, string departmentHeadId)
        {
            throw new NotImplementedException();
        }

        public Task AssignConsultants(int departmentId, List<ConsultantInDepartmentViewModel> consultantList)
        {
            throw new NotImplementedException();
        }

        public async Task CreateDepartment(Department department, string departmentHeadId)
        {
            return;
        }

        public async Task<List<DepartmentWithHeadViewModel>> CreateDepartmentWithHeadViewModelList(List<Department> departmentList)
        {
            var model = new List<DepartmentWithHeadViewModel>();
            return model;
        }

        public async Task DeleteAsync(int id)
        {
            return;
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return new List<Department>();
        }

        public async Task<List<IdentityUser>> GetAllConsultantsAndDepartmentHeads()
        {
            var model = new List<IdentityUser>();
            return model;
        }

        public Task<List<ConsultantInDepartmentViewModel>> GetAvailableConsultants(int departmentId)
        {
            throw new NotImplementedException();
        }

        public async Task<Department> GetByIdAsync(int id)
        {
            var department = new Department();
            return department;
        }

        public Task<List<Department>> GetDepartmentsUserIsAHeadOfAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Topic>> GetTopicsInDepartment(int departmentId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Department>> GetUserDepartments(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Department department, string departmentHeadId)
        {
            return;
        }
    }
}
