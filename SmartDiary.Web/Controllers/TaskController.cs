using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartDiary.Web.Data;
using SmartDiary.Web.Repositories.Services;
using TaskModel = SmartDiary.Web.Models.Task;

namespace SmartDiary.Web.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly ApplicationDbContext _context;

        public TaskController(ITaskService taskService, ApplicationDbContext context)
        {
            _taskService = taskService;
            _context = context;
        }

        // GET: /Task
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tasks = await _taskService.GetUserTasksAsync(userId);
            return View(tasks);
        }

        // GET: /Task/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var task = await _taskService.GetTaskByIdAsync(id, userId);
            if (task == null) return NotFound();
            return View(task);
        }

        // GET: /Task/Create
        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await LoadViewBags(userId);
            return View();
        }

        // POST: /Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskModel task, int[] selectedTags)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _taskService.CreateTaskAsync(task, userId, selectedTags ?? Array.Empty<int>());
                return RedirectToAction(nameof(Index));
            }

            // Используем одну переменную userId
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await LoadViewBags(currentUserId, task.ProjectId);
            return View(task);
        }

        // GET: /Task/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var task = await _taskService.GetTaskByIdAsync(id, userId);
            if (task == null) return NotFound();
            await LoadViewBags(userId, task.ProjectId);
            ViewBag.SelectedTags = task.TaskTags.Select(tt => tt.TagId).ToArray();
            return View(task);
        }

        // POST: /Task/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskModel task, int[] selectedTags)
        {
            if (id != task.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var updated = await _taskService.UpdateTaskAsync(task, userId, selectedTags ?? Array.Empty<int>());
                if (updated == null) return NotFound();
                return RedirectToAction(nameof(Index));
            }

            // Используем одну переменную currentUserId
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await LoadViewBags(currentUserId, task.ProjectId);
            return View(task);
        }

        // POST: /Task/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _taskService.DeleteTaskAsync(id, userId);
            if (!success) return NotFound();
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadViewBags(string userId, int? selectedProjectId = null)
        {
            ViewBag.Projects = new SelectList(
                await _context.Projects.Where(p => p.OwnerId == userId).ToListAsync(),
                "Id", "Name", selectedProjectId);

            ViewBag.Tags = await _context.Tags
                .Where(t => t.OwnerId == userId)
                .ToListAsync();
        }
    }
}