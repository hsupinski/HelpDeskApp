using HelpDeskApp.Models.Domain;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using HelpDeskApp.Models.ViewModels;

namespace HelpDeskApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TopicController : Controller
    {
        private readonly ITopicService _topicService;
        private readonly IDepartmentService _departmentService;
        public TopicController(ITopicService topicService, IDepartmentService departmentService)
        {
            _topicService = topicService;
            _departmentService = departmentService;
        }

        public async Task<IActionResult> Index()
        {
            var topicList = new List<Topic>();
            topicList = await _topicService.GetAllAsync();

            List<TopicViewModel> topicViewModelList = new List<TopicViewModel>();

            foreach (var topic in topicList)
            {
                var departmentNames = new List<string>();

                foreach (var departmentId in topic.DepartmentIds)
                {
                    var department = await _departmentService.GetByIdAsync(departmentId);
                    departmentNames.Add(department.Name);
                }

                var topicViewModel = new TopicViewModel
                {
                    Id = topic.Id,
                    Name = topic.Name,
                    DepartmentNames = departmentNames
                };

                topicViewModelList.Add(topicViewModel);
            }

            return View(topicViewModelList);
        }

        public async Task<IActionResult> Create()
        {
            var departments = await _departmentService.GetAllAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Topic topic)
        {
            await _topicService.AddAsync(topic);
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var topic = await _topicService.GetByIdAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            var departments = await _departmentService.GetAllAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");
            return View(topic);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Topic topic)
        {
            if (id != topic.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _topicService.UpdateAsync(topic);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = await _departmentService.GetAllAsync();
            return View(topic);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var topic = await _topicService.GetByIdAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            var topicViewModel = new TopicViewModel
            {
                Id = topic.Id,
                Name = topic.Name,
                DepartmentNames = new List<string>()
            };

            foreach (var departmentId in topic.DepartmentIds)
            {
                var department = await _departmentService.GetByIdAsync(departmentId);
                topicViewModel.DepartmentNames.Add(department.Name);
            }

            return View(topicViewModel);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _topicService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
