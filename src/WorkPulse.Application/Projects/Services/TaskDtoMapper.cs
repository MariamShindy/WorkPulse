using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Services;

internal static class TaskDtoMapper
{
	internal static async Task<TaskItemDto> MapTaskDtoAsync(IApplicationDbContext context, TaskItem task, CancellationToken ct)
	{
		// Build from the in-memory entity — Create/Update/Move map before SaveChanges,
		// so AsNoTracking queries against TaskItems would miss newly added rows.
		string teamKey = await context.Teams.AsNoTracking()
			.Where(team => team.Id == task.TeamId)
			.Select(team => team.Key)
			.FirstAsync(ct);

		var state = await context.WorkflowStates.AsNoTracking()
			.Where(workflowState => workflowState.Id == task.WorkflowStateId)
			.Select(workflowState => new { workflowState.Name, workflowState.Type })
			.FirstAsync(ct);

		string? projectKey = null;
		if (task.ProjectId.HasValue)
		{
			projectKey = await context.Projects.AsNoTracking()
				.Where(project => project.Id == task.ProjectId.Value)
				.Select(project => project.Key)
				.FirstOrDefaultAsync(ct);
		}

		List<Guid> assigneeIds = await ResolveAssigneeIdsAsync(context, task.Id, ct);

		return new TaskItemDto(
			task.Id,
			task.TeamId,
			teamKey,
			string.Concat(teamKey, "-", task.Number),
			task.ProjectId,
			projectKey,
			task.WorkflowStateId,
			state.Name,
			state.Type.ToString(),
			task.Title,
			task.Description,
			task.Priority.ToString(),
			task.AssigneeId,
			assigneeIds,
			task.CreatorId,
			task.DueDate,
			task.ParentTaskId,
			task.SortOrder,
			task.StoryPoints,
			task.EstimatedHours,
			task.LoggedHours,
			task.IsBlocked,
			task.BlockedReason,
			task.EpicId,
			task.SprintId,
			task.AssignedTeamId,
			task.CreatedAtUtc);
	}

	private static async Task<List<Guid>> ResolveAssigneeIdsAsync(IApplicationDbContext context, Guid taskId, CancellationToken ct)
	{
		List<Guid> fromStore = await context.TaskAssignees.AsNoTracking()
			.Where(assignee => assignee.TaskId == taskId)
			.Select(assignee => assignee.UserId)
			.ToListAsync(ct);

		IEnumerable<Guid> fromLocal = context.TaskAssignees.Local
			.Where(assignee => assignee.TaskId == taskId)
			.Select(assignee => assignee.UserId);

		return fromStore.Concat(fromLocal).Distinct().ToList();
	}

	internal static async Task SyncAssigneesAsync(IApplicationDbContext context, Guid tenantId, Guid taskId, IReadOnlyList<Guid>? assigneeIds, CancellationToken ct)
	{
		if (assigneeIds == null)
		{
			return;
		}

		List<TaskAssignee> existing = await context.TaskAssignees.Where(assignee => assignee.TaskId == taskId).ToListAsync(ct);
		HashSet<Guid> desired = assigneeIds.Distinct().ToHashSet();
		List<TaskAssignee> toRemove = existing.Where(assignee => !desired.Contains(assignee.UserId)).ToList();
		HashSet<Guid> existingUserIds = existing.Select(assignee => assignee.UserId).ToHashSet();
		context.TaskAssignees.RemoveRange(toRemove);

		foreach (Guid userId in desired.Where(id => !existingUserIds.Contains(id)))
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
