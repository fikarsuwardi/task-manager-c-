using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Domain.Entities
{
    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        public Guid AssignedToUserId { get; set; }
        public Guid ProjectId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        
        public virtual User AssignedToUser { get; set; }
        public virtual Project Project { get; set; }
    }

    public enum TaskStatus
    {
        Pending = 0,
        InProgress = 1,
        Done = 2
    }
}