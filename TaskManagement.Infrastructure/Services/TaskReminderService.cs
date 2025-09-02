using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Services
{
    public class TaskReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TaskReminderService> _logger;

        public TaskReminderService(IServiceProvider serviceProvider, ILogger<TaskReminderService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckTaskReminders();
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task CheckTaskReminders()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var upcomingTasks = await context.Tasks
                .Include(t => t.AssignedToUser)
                .Include(t => t.Project)
                .Where(t => t.DueDate.HasValue && 
                           t.DueDate.Value <= DateTime.UtcNow.AddDays(1) && 
                           t.Status != Domain.Entities.TaskStatus.Done)
                .ToListAsync();

            foreach (var task in upcomingTasks)
            {
                _logger.LogInformation($"Task reminder: {task.Title} for {task.AssignedToUser.Username} due {task.DueDate}");
                // Here you would send email/notification
            }
        }
    }
}