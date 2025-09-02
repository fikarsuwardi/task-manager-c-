using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var query = _context.Projects.Include(p => p.CreatedByUser).AsQueryable();

            if (userRole == "Manager")
                query = query.Where(p => p.CreatedByUserId == userId);

            var projects = await query.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedByUserId = p.CreatedByUserId,
                CreatedByUsername = p.CreatedByUser.Username,
                CreatedAt = p.CreatedAt
            }).ToListAsync();

            return Ok(projects);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedByUserId = userId
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return Ok(new { Id = project.Id, Message = "Project created successfully" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(Guid id, [FromBody] CreateProjectDto dto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != "Admin" && project.CreatedByUserId != userId)
                return Forbid();

            project.Name = dto.Name;
            project.Description = dto.Description;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Project updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole != "Admin" && project.CreatedByUserId != userId)
                return Forbid();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Project deleted successfully" });
        }
    }
}