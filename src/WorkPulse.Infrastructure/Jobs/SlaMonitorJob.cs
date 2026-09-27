using WorkPulse.Application.Collaboration.Services;

namespace WorkPulse.Infrastructure.Jobs;

public sealed class SlaMonitorJob(ApplicationDbContext db, ITaskCollaborationService collaboration, ILogger<SlaMonitorJob> logger)
{
	public async Task RunAsync(CancellationToken ct = default(CancellationToken))
	{
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
		var overdueTasks = await (from t in db.TaskItems.IgnoreQueryFilters()
			join team in db.Teams.IgnoreQueryFilters() on t.TeamId equals team.Id
			join state in db.WorkflowStates.IgnoreQueryFilters() on t.WorkflowStateId equals state.Id
			where !t.IsDeleted && t.DueDate != null && t.DueDate < today && (int)state.Type != 3 && (int)state.Type != 4
			select new { t, team }).ToListAsync(ct);
		foreach (var item in overdueTasks)
		{
			if (item.t.AssigneeId.HasValue)
			{
				string identifier = $"{item.team.Key}-{item.t.Number}";
				await collaboration.NotifyUsersAsync(item.t.TenantId, [item.t.AssigneeId.Value], NotificationType.SlaViolation, "SLA breach: " + identifier, $"Task {identifier} is overdue (due {item.t.DueDate:yyyy-MM-dd}).", null, "TaskItem", item.t.Id, ct);
				logger.LogWarning("SLA violation detected for task {TaskId}", item.t.Id);
			}
		}
		if (overdueTasks.Count > 0)
		{
			await db.SaveChangesAsync(ct);
		}
	}
}
