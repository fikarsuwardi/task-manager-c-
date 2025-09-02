using TaskStatus = TaskManagement.Domain.Entities.TaskStatus;

namespace TaskManagement.Application.DTOs
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
        public Guid AssignedToUserId { get; set; }
        public string AssignedToUsername { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class CreateTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid AssignedToUserId { get; set; }
        public Guid ProjectId { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class UpdateTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime? DueDate { get; set; }
    }
}