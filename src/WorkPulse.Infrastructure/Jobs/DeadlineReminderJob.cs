using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Collaboration.Services;
using WorkPulse.Domain.Entities;
using WorkPulse.Domain.Enums;
using WorkPulse.Infrastructure.Persistence;

namespace WorkPulse.Infrastructure.Jobs;

public sealed class DeadlineReminderJob(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IEmailService emailService, ITaskCollaborationService collaboration, ILogger<DeadlineReminderJob> logger)
{
	public async Task RunAsync(CancellationToken ct = default(CancellationToken))
	{
		DateOnly tomorrow = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);
		var dueTasks = await (from t in db.TaskItems.IgnoreQueryFilters()
			join team in db.Teams.IgnoreQueryFilters() on t.TeamId equals team.Id
			join state in db.WorkflowStates.IgnoreQueryFilters() on t.WorkflowStateId equals state.Id
			where !t.IsDeleted && t.DueDate == tomorrow && (int)state.Type != 3 && (int)state.Type != 4 && t.AssigneeId != null
			select new { t, team }).ToListAsync(ct);
		foreach (var item in dueTasks)
		{
			Guid assigneeId = item.t.AssigneeId.Value;
			ApplicationUser user = await userManager.FindByIdAsync(assigneeId.ToString());
			if (user?.Email != null)
			{
				string identifier = $"{item.team.Key}-{item.t.Number}";
				string subject = "Reminder: " + identifier + " is due tomorrow";
				string body = $"<p>Task <strong>{identifier}</strong> ({item.t.Title}) is due on {item.t.DueDate:yyyy-MM-dd}.</p>";
				await emailService.SendAsync(user.Email, subject, body, ct);
				await collaboration.NotifyUsersAsync(item.t.TenantId, [assigneeId], NotificationType.DeadlineReminder, subject, "Task " + identifier + " is due tomorrow.", null, "TaskItem", item.t.Id, ct);
				logger.LogInformation("Sent deadline reminder for task {TaskId} to {UserId}", item.t.Id, assigneeId);
			}
		}
		if (dueTasks.Count > 0)
		{
			await db.SaveChangesAsync(ct);
		}
	}
}
