using HelpDeskApp.Models.Domain;
using HelpDeskApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            return View(topicList);
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

            ViewBag.Departments = await _departmentService.GetAllAsync();
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

            return View(topic);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _topicService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
