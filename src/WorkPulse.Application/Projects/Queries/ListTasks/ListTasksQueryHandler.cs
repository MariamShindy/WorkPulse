using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Queries.ListTasks;

public sealed class ListTasksQueryHandler(IApplicationDbContext context, ITenantContext tenantContext)
	: IRequestHandler<ListTasksQuery, Result<PagedList<TaskItemDto>>>
{
	public async Task<Result<PagedList<TaskItemDto>>> Handle(ListTasksQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}

		var query =
			from task in context.TaskItems.AsNoTracking().ForTenant(tenantContext)
			join team in context.Teams.AsNoTracking().ForTenant(tenantContext) on task.TeamId equals team.Id
			join state in context.WorkflowStates.AsNoTracking().ForTenant(tenantContext) on task.WorkflowStateId equals state.Id
			join project in context.Projects.AsNoTracking().ForTenant(tenantContext) on task.ProjectId equals project.Id into projects
			from project in projects.DefaultIfEmpty()
			select new { task, team, state, project };

		if (request.TeamId.HasValue)
		{
			query = query.Where(row => row.task.TeamId == request.TeamId.Value);
		}

		if (request.ProjectId.HasValue)
		{
			query = query.Where(row => row.task.ProjectId == request.ProjectId.Value);
		}

		if (request.AssigneeId.HasValue)
		{
			Guid assigneeId = request.AssigneeId.Value;
			query = query.Where(row =>
				row.task.AssigneeId == assigneeId ||
				context.TaskAssignees.Any(assignee => assignee.TaskId == row.task.Id && assignee.UserId == assigneeId));
		}

		if (request.WorkflowStateId.HasValue)
		{
			query = query.Where(row => row.task.WorkflowStateId == request.WorkflowStateId.Value);
		}

		if (request.Priority.HasValue)
		{
			query = query.Where(row => row.task.Priority == request.Priority.Value);
		}

		query = query
			.OrderBy(row => row.task.SortOrder)
			.ThenByDescending(row => row.task.CreatedAtUtc);

		int total = await query.CountAsync(ct);
		List<Guid> pageTaskIds = await query
			.Skip(request.Pagination.Skip)
			.Take(request.Pagination.PageSize)
			.Select(row => row.task.Id)
			.ToListAsync(ct);

		if (pageTaskIds.Count == 0)
		{
			return new PagedList<TaskItemDto>([], request.Pagination.Page, request.Pagination.PageSize, total);
		}

		Dictionary<Guid, IReadOnlyList<Guid>> assigneesByTask = await context.TaskAssignees.AsNoTracking()
			.Where(assignee => pageTaskIds.Contains(assignee.TaskId))
			.GroupBy(assignee => assignee.TaskId)
			.Select(group => new
			{
				TaskId = group.Key,
				UserIds = group.Select(assignee => assignee.UserId).ToList()
			})
			.ToDictionaryAsync(group => group.TaskId, group => (IReadOnlyList<Guid>)group.UserIds, ct);

		Dictionary<Guid, TaskItemDto> tasksById = (await query
			.Where(row => pageTaskIds.Contains(row.task.Id))
			.Select(row => new TaskItemDto(
				row.task.Id,
				row.task.TeamId,
				row.team.Key,
				string.Concat(row.team.Key + "-", row.task.Number),
				row.task.ProjectId,
				row.project != null ? row.project.Key : null,
				row.task.WorkflowStateId,
				row.state.Name,
				row.state.Type.ToString(),
				row.task.Title,
				row.task.Description,
				row.task.Priority.ToString(),
				row.task.AssigneeId,
				Array.Empty<Guid>(),
				row.task.CreatorId,
				row.task.DueDate,
				row.task.ParentTaskId,
				row.task.SortOrder,
				row.task.StoryPoints,
				row.task.EstimatedHours,
				row.task.LoggedHours,
				row.task.IsBlocked,
				row.task.BlockedReason,
				row.task.EpicId,
				row.task.SprintId,
				row.task.AssignedTeamId,
				row.task.CreatedAtUtc))
			.ToListAsync(ct))
			.Select(dto => dto with
			{
				AssigneeIds = assigneesByTask.GetValueOrDefault(dto.Id, Array.Empty<Guid>())
			})
			.ToDictionary(dto => dto.Id);

		List<TaskItemDto> ordered = pageTaskIds
			.Where(tasksById.ContainsKey)
			.Select(taskId => tasksById[taskId])
			.ToList();

		return new PagedList<TaskItemDto>(ordered, request.Pagination.Page, request.Pagination.PageSize, total);
	}
}
