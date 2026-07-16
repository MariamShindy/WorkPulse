using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Application.WorkManagement.Dtos;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.WorkManagement.Queries.GetSprintBacklog;

public sealed class GetSprintBacklogQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<GetSprintBacklogQuery, Result<PagedList<TaskItemDto>>>
{
	public async Task<Result<PagedList<TaskItemDto>>> Handle(GetSprintBacklogQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		if (!(await context.Teams.AnyAsync((Team t) => t.Id == request.TeamId, ct)))
		{
			return Error.NotFound("Team.NotFound", "Team not found.");
		}
		bool hasValue = request.SprintId.HasValue;
		bool flag = hasValue;
		if (flag)
		{
			flag = !(await context.Sprints.AnyAsync((Sprint s) => s.Id == request.SprintId && s.TeamId == request.TeamId, ct));
		}
		if (flag)
		{
			return Error.NotFound("Sprint.NotFound", "Sprint not found.");
		}
		var query = from t in context.TaskItems.AsNoTracking()
			join team in context.Teams.AsNoTracking() on t.TeamId equals team.Id
			join state in context.WorkflowStates.AsNoTracking() on t.WorkflowStateId equals state.Id
			join project in context.Projects.AsNoTracking() on t.ProjectId equals project.Id into projects
			from project in projects.DefaultIfEmpty()
			where t.TeamId == request.TeamId
			select new { t, team, state, project };
		query = (request.SprintId.HasValue ? query.Where(x => x.t.SprintId == request.SprintId.Value) : query.Where(x => x.t.SprintId == null));
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
