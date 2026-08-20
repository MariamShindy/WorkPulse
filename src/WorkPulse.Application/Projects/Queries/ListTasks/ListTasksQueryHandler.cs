using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Queries.ListTasks;

public sealed class ListTasksQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ListTasksQuery, Result<PagedList<TaskItemDto>>>
{
	public async Task<Result<PagedList<TaskItemDto>>> Handle(ListTasksQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		var query = from t in context.TaskItems.AsNoTracking().ForTenant(tenantContext)
			join team in context.Teams.AsNoTracking().ForTenant(tenantContext) on t.TeamId equals team.Id
			join state in context.WorkflowStates.AsNoTracking().ForTenant(tenantContext) on t.WorkflowStateId equals state.Id
			join project in context.Projects.AsNoTracking().ForTenant(tenantContext) on t.ProjectId equals project.Id into projects
			from project in projects.DefaultIfEmpty()
			select new { t, team, state, project };
		if (request.TeamId.HasValue)
		{
			query = query.Where(x => x.t.TeamId == request.TeamId.Value);
		}
		if (request.ProjectId.HasValue)
		{
			query = query.Where(x => x.t.ProjectId == request.ProjectId.Value);
		}
		if (request.AssigneeId.HasValue)
		{
			query = query.Where(x => x.t.AssigneeId == request.AssigneeId.Value);
		}
		if (request.WorkflowStateId.HasValue)
		{
			query = query.Where(x => x.t.WorkflowStateId == request.WorkflowStateId.Value);
		}
		if (request.Priority.HasValue)
		{
			query = query.Where(x => (int)x.t.Priority == (int)request.Priority.Value);
		}
		query = from x in query
			orderby x.t.SortOrder, x.t.CreatedAtUtc descending
			select x;
		int total = await query.CountAsync(ct);
		List<Guid> taskIds = await (from x in query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize)
			select x.t.Id).ToListAsync(ct);
		Dictionary<Guid, IReadOnlyList<Guid>> assigneesByTask = await (from a in context.TaskAssignees.AsNoTracking()
			where taskIds.Contains(a.TaskId)
			group a by a.TaskId into g
			select new
			{
				TaskId = g.Key,
				UserIds = g.Select((TaskAssignee a) => a.UserId).ToList()
			}).ToDictionaryAsync(x => x.TaskId, x => (IReadOnlyList<Guid>)x.UserIds, ct);
		List<TaskItemDto> result = (await (from x in query
			where taskIds.Contains(x.t.Id)
			select new TaskItemDto(x.t.Id, x.t.TeamId, x.team.Key, string.Concat(x.team.Key + "-", x.t.Number), x.t.ProjectId, (x.project != null) ? x.project.Key : null, x.t.WorkflowStateId, x.state.Name, x.state.Type.ToString(), x.t.Title, x.t.Description, x.t.Priority.ToString(), x.t.AssigneeId, Array.Empty<Guid>(), x.t.CreatorId, x.t.DueDate, x.t.ParentTaskId, x.t.SortOrder, x.t.StoryPoints, x.t.EstimatedHours, x.t.LoggedHours, x.t.IsBlocked, x.t.BlockedReason, x.t.EpicId, x.t.SprintId, x.t.AssignedTeamId, x.t.CreatedAtUtc)).ToListAsync(ct)).Select(delegate(TaskItemDto dto)
		{
			IReadOnlyList<Guid> valueOrDefault = assigneesByTask.GetValueOrDefault(dto.Id, Array.Empty<Guid>());
			return dto with
			{
				AssigneeIds = valueOrDefault
			};
		}).ToList();
		return new PagedList<TaskItemDto>(result, request.Pagination.Page, request.Pagination.PageSize, total);
	}
}
