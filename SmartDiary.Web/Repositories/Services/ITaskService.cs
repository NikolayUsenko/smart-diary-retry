using Task = SmartDiary.Web.Models.Task;

namespace SmartDiary.Web.Repositories.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<Task>> GetUserTasksAsync(string userId);
        Task<Task?> GetTaskByIdAsync(int id, string userId);
        Task<Task> CreateTaskAsync(Task task, string userId, int[] selectedTags);
        Task<Task?> UpdateTaskAsync(Task task, string userId, int[] selectedTags);
        Task<bool> DeleteTaskAsync(int id, string userId);
    }
}
