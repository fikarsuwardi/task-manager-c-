using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public virtual User CreatedByUser { get; set; }
        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}