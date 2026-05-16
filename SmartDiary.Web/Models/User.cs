using Azure;
using Microsoft.AspNetCore.Identity;
using SmartDiary.Web.Models;
namespace SmartDiary.Web.Models
{
    public class User : IdentityUser
    {
        public string? Avatar { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Settings { get; set; }
        // Навигационные свойства
        public ICollection<Project> Projects { get; set; } = new
        List<Project>();
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
