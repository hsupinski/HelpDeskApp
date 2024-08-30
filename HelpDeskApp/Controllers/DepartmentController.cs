using HelpDeskApp.Models.Domain;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HelpDeskApp.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private readonly IAccountService _accountService;
        private readonly ITopicService _topicService;

        public DepartmentController(IDepartmentService departmentService, IAccountService accountService, ITopicService topicService)
        {
            _departmentService = departmentService;
            _accountService = accountService;
            _topicService = topicService;
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

            var redirectTopic = new Topic();
            redirectTopic.Name = "Redirect to: " + department.Name;
            redirectTopic.DepartmentIds = new List<int> { department.Id };

            await _topicService.AddAsync(redirectTopic);

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
            await _topicService.DeleteTopicsWithoutDepartment();

            return RedirectToAction(nameof(Index));
        }
    }
}
