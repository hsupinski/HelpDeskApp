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

            var model = await _departmentService.CreateDepartmentWithHeadViewModelList(departmentList);

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var users = await _departmentService.GetAllConsultantsAndDepartmentHeads();
           
            ViewBag.Users = users;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Department department, string departmentHeadId)
        {
            await _departmentService.CreateDepartment(department, departmentHeadId);
            
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            var users = await _departmentService.GetAllConsultantsAndDepartmentHeads();

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

            ViewBag.Users = await _departmentService.GetAllConsultantsAndDepartmentHeads();

            return View(department);
        }

        public async Task<IActionResult> AssignConsultants(int id)
        {
            var model = await _departmentService.GetAvailableConsultants(id);

            ViewBag.DepartmentId = id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignConsultants(int id, List<ConsultantInDepartmentViewModel> model)
        {
            await _departmentService.AssignConsultants(id, model);

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
