using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Services;

internal static class TaskDtoMapper
{
	internal static async Task<TaskItemDto> MapTaskDtoAsync(IApplicationDbContext context, TaskItem task, CancellationToken ct)
	{
		List<Guid> assigneeIds = await (from a in context.TaskAssignees.AsNoTracking()
			where a.TaskId == task.Id
			select a.UserId).ToListAsync(ct);
		TaskItemDto dto = await (from t in context.TaskItems.AsNoTracking()
			join team in context.Teams.AsNoTracking() on t.TeamId equals team.Id
			join state in context.WorkflowStates.AsNoTracking() on t.WorkflowStateId equals state.Id
			join project in context.Projects.AsNoTracking() on t.ProjectId equals project.Id into projects
			from project in projects.DefaultIfEmpty()
			where t.Id == task.Id
			select new TaskItemDto(t.Id, t.TeamId, team.Key, string.Concat(team.Key + "-", t.Number), t.ProjectId, (project != null) ? project.Key : null, t.WorkflowStateId, state.Name, state.Type.ToString(), t.Title, t.Description, t.Priority.ToString(), t.AssigneeId, Array.Empty<Guid>(), t.CreatorId, t.DueDate, t.ParentTaskId, t.SortOrder, t.StoryPoints, t.EstimatedHours, t.LoggedHours, t.IsBlocked, t.BlockedReason, t.EpicId, t.SprintId, t.AssignedTeamId, t.CreatedAtUtc)).FirstAsync(ct);
		return dto with
		{
			AssigneeIds = assigneeIds
		};
	}

	internal static async Task SyncAssigneesAsync(IApplicationDbContext context, Guid tenantId, Guid taskId, IReadOnlyList<Guid>? assigneeIds, CancellationToken ct)
	{
		if (assigneeIds == null)
		{
			return;
		}
		List<TaskAssignee> existing = await context.TaskAssignees.Where((TaskAssignee a) => a.TaskId == taskId).ToListAsync(ct);
		HashSet<Guid> desired = assigneeIds.Distinct().ToHashSet();
		List<TaskAssignee> toRemove = existing.Where((TaskAssignee a) => !desired.Contains(a.UserId)).ToList();
		HashSet<Guid> existingUserIds = existing.Select((TaskAssignee a) => a.UserId).ToHashSet();
		context.TaskAssignees.RemoveRange(toRemove);
		foreach (Guid userId in desired.Where((Guid id) => !existingUserIds.Contains(id)))
		{
			context.TaskAssignees.Add(new TaskAssignee
			{
				Id = Guid.NewGuid(),
				TenantId = tenantId,
				TaskId = taskId,
				UserId = userId
			});
		}
	}
}
