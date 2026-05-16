using Microsoft.EntityFrameworkCore;
using SmartDiary.Web.Data;
using SmartDiary.Web.Models;
using SmartDiary.Web.Repositories.Services;
using Task = SmartDiary.Web.Models.Task;

namespace SmartDiary.Web.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;
        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Task>> GetUserTasksAsync(string userId)
        {
            return await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.TaskTags)
            .ThenInclude(tt => tt.Tag)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
        }
        public async Task<Task?> GetTaskByIdAsync(int id, string userId)
        {
            return await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.TaskTags)
            .ThenInclude(tt => tt.Tag)
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }
        public async Task<Task> CreateTaskAsync(Task task, string userId, int[]
        selectedTags)
        {
            task.UserId = userId;
            task.CreatedAt = DateTime.UtcNow;
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            if (selectedTags != null && selectedTags.Any())
            {
                foreach (var tagId in selectedTags)
                {
                    _context.TaskTags.Add(new TaskTag
                    {
                        TaskId = task.Id,
                        TagId = tagId
                    });
                }
                await _context.SaveChangesAsync();
            }
            return task;
        }
        public async Task<Task?> UpdateTaskAsync(Task task, string userId, int[]
        selectedTags)
        {
            var existingTask = await _context.Tasks
            .Include(t => t.TaskTags)
            .FirstOrDefaultAsync(t => t.Id == task.Id && t.UserId == userId);
            if (existingTask == null) return null;
            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.Deadline = task.Deadline;
            existingTask.Status = task.Status;
            existingTask.Priority = task.Priority;
            existingTask.ProjectId = task.ProjectId;
            // Обновление тегов
            existingTask.TaskTags.Clear();
            if (selectedTags != null && selectedTags.Any())
            {
                foreach (var tagId in selectedTags)
                {
                    existingTask.TaskTags.Add(new TaskTag
                    {
                        TaskId = task.Id,
                        TagId = tagId
                    });
                }
            }
            await _context.SaveChangesAsync();
            return existingTask;
        }
        public async Task<bool> DeleteTaskAsync(int id, string userId)
        {
            var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (task == null) return false;
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
