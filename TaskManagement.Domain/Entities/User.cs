using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }

    public enum UserRole
    {
        Admin = 0,
        Manager = 1,
        User = 2
    }
}