using HelpDeskApp.Models.Domain;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HelpDeskApp.Models.ViewModels;

namespace HelpDeskApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly IAccountService _accountService;

        public DepartmentController(IDepartmentService departmentService, IAccountService accountService)
        {
            _departmentService = departmentService;
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            List<Department> departmentList = await _departmentService.GetAllAsync();

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

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            Console.WriteLine("Enter Create Department");

            var consultantList = await _accountService.GetUsersInRoleAsync("Consultant");
            var departmentHeadList = await _accountService.GetUsersInRoleAsync("Department Head");

            var users = new List<IdentityUser>();
            users.AddRange(consultantList);
            users.AddRange(departmentHeadList);

            users = users.Distinct().ToList(); // remove duplicates

            ViewBag.Users = users;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Department department, string departmentHeadId)
        {
            Console.WriteLine("Create Department");

            var user = await _accountService.GetUserByIdAsync(departmentHeadId);
            var userRoles = await _accountService.GetUserRolesAsync(user);

            // If selected user is not a Department Head, add the role
            if (!userRoles.Contains("Department Head"))
            {
                await _accountService.AddUserToRolesAsync(user, new List<string> { "Department Head" });
            }
   
            await _departmentService.AddAsync(department, departmentHeadId);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);

            if (department == null)
            {
                return NotFound();
            }
            
            var consultantList = await _accountService.GetUsersInRoleAsync("Consultant");
            var departmentHeadList = await _accountService.GetUsersInRoleAsync("Department Head");

            var users = new List<IdentityUser>();
            
            users.AddRange(consultantList);
            users.AddRange(departmentHeadList);

            users = users.Distinct().ToList(); // remove duplicates

            ViewBag.Users = users;

            return View(department);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Department department, string departmentHeadId)
        {
            if (id != department.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _departmentService.UpdateAsync(department, departmentHeadId);
                return RedirectToAction("Index");
            }

            ViewBag.Users = await _accountService.GetUsersInRoleAsync("Consultant");
            ViewBag.Users.AddRange(await _accountService.GetUsersInRoleAsync("Department Head"));
            return View(department);
        }

        public async Task<IActionResult> AssignConsultants(int id)
        {
            var consultants = await _accountService.GetUsersInRoleAsync("Consultant");
            var departmentHeads = await _accountService.GetUsersInRoleAsync("Department Head");
            var department = await _departmentService.GetByIdAsync(id);

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
                    model.Add(new ConsultantInDepartmentViewModel
                    {
                        ConsultantId = user.Id,
                        ConsultantName = user.UserName,
                        IsInDepartment = department.ConsultantId.Contains(user.Id)
                    });
                }
            }

            ViewBag.DepartmentId = id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignConsultants(int id, List<ConsultantInDepartmentViewModel> model)
        {
            var department = await _departmentService.GetByIdAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            var consultants = new List<string>();

            foreach (var consultant in model)
            {
                if (consultant.IsInDepartment)
                {
                    consultants.Add(consultant.ConsultantId);
                }
            }

            department.ConsultantId = consultants;
            await _departmentService.UpdateAsync(department, department.DepartmentHeadId);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _departmentService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
