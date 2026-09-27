using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Queries.GetSprintBacklog;

public sealed class GetSprintBacklogQueryHandler(
	IApplicationDbContext context,
	ITenantContext tenantContext) : IRequestHandler<GetSprintBacklogQuery, Result<PagedList<TaskItemDto>>>
{
	public async Task<Result<PagedList<TaskItemDto>>> Handle(
		GetSprintBacklogQuery request,
		CancellationToken cancellationToken)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}

		if (!await context.Teams.AsNoTracking().ForTenant(tenantContext).AnyAsync(team => team.Id == request.TeamId, cancellationToken))
		{
			return Error.NotFound(TeamErrors.NotFoundCode, "Team not found.");
		}

		if (request.SprintId.HasValue &&
		    !await context.Sprints.AsNoTracking().ForTenant(tenantContext).AnyAsync(
			    sprint => sprint.Id == request.SprintId && sprint.TeamId == request.TeamId,
			    cancellationToken))
		{
			return Error.NotFound(SprintErrors.NotFoundCode, "Sprint not found.");
		}

		var backlogQuery = BuildBacklogQuery(request);
		int totalCount = await backlogQuery.CountAsync(cancellationToken);

		List<Guid> pageTaskIds = await backlogQuery
			.Skip(request.Pagination.Skip)
			.Take(request.Pagination.PageSize)
			.Select(row => row.Task.Id)
			.ToListAsync(cancellationToken);

		if (pageTaskIds.Count == 0)
		{
			return new PagedList<TaskItemDto>([], request.Pagination.Page, request.Pagination.PageSize, totalCount);
		}

		Dictionary<Guid, IReadOnlyList<Guid>> assigneesByTask = await LoadAssigneesByTaskAsync(pageTaskIds, cancellationToken);
		List<TaskItemDto> tasks = await MapTasksAsync(backlogQuery, pageTaskIds, assigneesByTask, cancellationToken);

		return new PagedList<TaskItemDto>(tasks, request.Pagination.Page, request.Pagination.PageSize, totalCount);
	}

	private IQueryable<BacklogRow> BuildBacklogQuery(GetSprintBacklogQuery request)
	{
		var query =
			from task in context.TaskItems.AsNoTracking().ForTenant(tenantContext)
			join team in context.Teams.AsNoTracking().ForTenant(tenantContext) on task.TeamId equals team.Id
			join state in context.WorkflowStates.AsNoTracking().ForTenant(tenantContext) on task.WorkflowStateId equals state.Id
			join project in context.Projects.AsNoTracking().ForTenant(tenantContext) on task.ProjectId equals project.Id into projects
			from project in projects.DefaultIfEmpty()
			where task.TeamId == request.TeamId
			select new BacklogRow(task, team, state, project);

		query = request.SprintId.HasValue
			? query.Where(row => row.Task.SprintId == request.SprintId.Value)
			: query.Where(row => row.Task.SprintId == null);

		return query
			.OrderBy(row => row.Task.SortOrder)
			.ThenByDescending(row => row.Task.CreatedAtUtc);
	}

	private async Task<Dictionary<Guid, IReadOnlyList<Guid>>> LoadAssigneesByTaskAsync(
		List<Guid> taskIds,
		CancellationToken cancellationToken)
	{
		return await context.TaskAssignees.AsNoTracking()
			.Where(assignee => taskIds.Contains(assignee.TaskId))
			.GroupBy(assignee => assignee.TaskId)
			.Select(group => new
			{
				TaskId = group.Key,
				UserIds = group.Select(assignee => assignee.UserId).ToList()
			})
			.ToDictionaryAsync(
				group => group.TaskId,
				group => (IReadOnlyList<Guid>)group.UserIds,
				cancellationToken);
	}

	private static async Task<List<TaskItemDto>> MapTasksAsync(
		IQueryable<BacklogRow> backlogQuery,
		List<Guid> pageTaskIds,
		Dictionary<Guid, IReadOnlyList<Guid>> assigneesByTask,
		CancellationToken cancellationToken)
	{
		List<TaskItemDto> tasks = await backlogQuery
			.Where(row => pageTaskIds.Contains(row.Task.Id))
			.Select(row => new TaskItemDto(
				row.Task.Id,
				row.Task.TeamId,
				row.Team.Key,
				string.Concat(row.Team.Key + "-", row.Task.Number),
				row.Task.ProjectId,
				row.Project != null ? row.Project.Key : null,
				row.Task.WorkflowStateId,
				row.State.Name,
				row.State.Type.ToString(),
				row.Task.Title,
				row.Task.Description,
				row.Task.Priority.ToString(),
				row.Task.AssigneeId,
				Array.Empty<Guid>(),
				row.Task.CreatorId,
				row.Task.DueDate,
				row.Task.ParentTaskId,
				row.Task.SortOrder,
				row.Task.StoryPoints,
				row.Task.EstimatedHours,
				row.Task.LoggedHours,
				row.Task.IsBlocked,
				row.Task.BlockedReason,
				row.Task.EpicId,
				row.Task.SprintId,
				row.Task.AssignedTeamId,
				row.Task.CreatedAtUtc))
			.ToListAsync(cancellationToken);

		Dictionary<Guid, TaskItemDto> tasksById = tasks
			.Select(task => task with
			{
				AssigneeIds = assigneesByTask.GetValueOrDefault(task.Id, Array.Empty<Guid>())
			})
			.ToDictionary(task => task.Id);

		return pageTaskIds
			.Where(tasksById.ContainsKey)
			.Select(taskId => tasksById[taskId])
			.ToList();
	}
}
