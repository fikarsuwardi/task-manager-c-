using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Services;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IExternalApiService _externalApi;

        public TasksController(ApplicationDbContext context, IExternalApiService externalApi)
        {
            _context = context;
            _externalApi = externalApi;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var query = _context.Tasks
                .Include(t => t.AssignedToUser)
                .Include(t => t.Project)
                .AsQueryable();

            if (userRole == "User")
                query = query.Where(t => t.AssignedToUserId == userId);
            else if (userRole == "Manager")
                query = query.Where(t => t.Project.CreatedByUserId == userId);

            var tasks = await query.Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                AssignedToUserId = t.AssignedToUserId,
                AssignedToUsername = t.AssignedToUser.Username,
                ProjectId = t.ProjectId,
                ProjectName = t.Project.Name,
                CreatedAt = t.CreatedAt,
                DueDate = t.DueDate
            }).ToListAsync();

            return Ok(tasks);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var description = string.IsNullOrEmpty(dto.Description) 
                    ? await _externalApi.GetRandomQuoteAsync() 
                    : dto.Description;

                var task = new TaskItem
                {
                    Title = dto.Title,
                    Description = description,
                    AssignedToUserId = dto.AssignedToUserId,
                    ProjectId = dto.ProjectId,
                    DueDate = dto.DueDate
                };

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { Id = task.Id, Message = "Task created successfully" });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto dto)
        {
            var task = await _context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == id);
            if (task == null) return NotFound();

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "User" && task.AssignedToUserId != userId)
                return Forbid();
            if (userRole == "Manager" && task.Project.CreatedByUserId != userId)
                return Forbid();

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Status = dto.Status;
            task.DueDate = dto.DueDate;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Task updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var task = await _context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == id);
            if (task == null) return NotFound();

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "User" && task.AssignedToUserId != userId)
                return Forbid();
            if (userRole != "Admin" && userRole != "Manager")
                return Forbid();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Task deleted successfully" });
        }
    }
}