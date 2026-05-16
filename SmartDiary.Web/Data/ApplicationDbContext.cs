using Azure;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartDiary.Web.Models;
using Task = SmartDiary.Web.Models.Task;

namespace SmartDiary.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>
        options)
        : base(options) { }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskTag> TaskTags { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Настройки уникальности
            builder.Entity<Tag>()
            .HasIndex(t => new { t.Name, t.OwnerId })
            .IsUnique();
            builder.Entity<Project>()
            .HasIndex(p => new { p.Name, p.OwnerId })
            .IsUnique();
        }
    }
}
