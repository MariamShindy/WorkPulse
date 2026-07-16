using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Reports.Dtos;
using WorkPulse.Domain.Entities;
using WorkPulse.Infrastructure.Persistence;

namespace WorkPulse.Infrastructure.Services.Read;

public sealed class ReportsReadService(ApplicationDbContext context) : IReportsReadService
{
	private sealed record TaskRowProjection(Guid TaskId, string Identifier, string Title, string TeamKey, string? ProjectKey, string Status, string Priority, Guid? AssigneeId, DateOnly? DueDate, DateTime CreatedAtUtc, Guid WorkflowStateId);

	public async Task<TaskSummaryReportDto> GetTaskSummaryReportAsync(Guid tenantId, ReportFilter filter, CancellationToken ct)
	{
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
		HashSet<Guid> completedSet = await GetCompletedStateIdSetAsync(tenantId, ct);
		List<TaskRowProjection> rows = await SelectTaskRows(tenantId, (from t in ApplyTaskFilters(tenantId, filter)
			orderby t.CreatedAtUtc descending
			select t).Take(5000)).ToListAsync(ct);
		return new TaskSummaryReportDto(DateTime.UtcNow, rows.Count, rows.Count((TaskRowProjection r) => !completedSet.Contains(r.WorkflowStateId)), rows.Count((TaskRowProjection r) => completedSet.Contains(r.WorkflowStateId)), rows.Count((TaskRowProjection r) => r.DueDate.HasValue && r.DueDate < today && !completedSet.Contains(r.WorkflowStateId)), rows.Select(MapRow).ToList());
	}

	public async Task<OverdueTasksReportDto> GetOverdueTasksReportAsync(Guid tenantId, ReportFilter filter, CancellationToken ct)
	{
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
		HashSet<Guid> completedSet = await GetCompletedStateIdSetAsync(tenantId, ct);
		List<TaskRowProjection> rows = await SelectTaskRows(tenantId, (from t in ApplyTaskFilters(tenantId, filter)
			where t.DueDate != null && t.DueDate < today && !completedSet.Contains(t.WorkflowStateId)
			orderby t.DueDate
			select t).Take(2000)).ToListAsync(ct);
		return new OverdueTasksReportDto(DateTime.UtcNow, rows.Count, rows.Select(MapRow).ToList());
	}

	public async Task<TeamPerformanceReportDto> GetTeamPerformanceReportAsync(Guid tenantId, ReportFilter filter, CancellationToken ct)
	{
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
		HashSet<Guid> completedSet = await GetCompletedStateIdSetAsync(tenantId, ct);
		IQueryable<Team> teams = from t in context.Teams.AsNoTracking()
			where t.TenantId == tenantId && !t.IsArchived
			select t;
		if (filter.TeamId.HasValue)
		{
			teams = teams.Where((Team t) => t.Id == filter.TeamId.Value);
		}
		var teamList = await teams.Select((Team t) => new { t.Id, t.Key, t.Name }).ToListAsync(ct);
		IQueryable<TaskItem> tasks = context.TaskItems.AsNoTracking().Where((TaskItem t) => t.TenantId == tenantId);
		if (filter.TeamId.HasValue)
		{
			tasks = tasks.Where((TaskItem t) => t.TeamId == filter.TeamId.Value);
		}
		var taskRows = await tasks.Select((TaskItem t) => new { t.TeamId, t.WorkflowStateId, t.DueDate, t.CreatedAtUtc, t.UpdatedAtUtc }).ToListAsync(ct);
		return new TeamPerformanceReportDto(Teams: (from t in teamList.Select(team =>
			{
				var list = taskRows.Where(t => t.TeamId == team.Id).ToList();
				int count = list.Count;
				int num = list.Count(t => completedSet.Contains(t.WorkflowStateId));
				int overdueTasks = list.Count(t => t.DueDate.HasValue && t.DueDate < today && !completedSet.Contains(t.WorkflowStateId));
				List<double> list2 = (from t in list
					where completedSet.Contains(t.WorkflowStateId) && t.UpdatedAtUtc.HasValue
					select (t.UpdatedAtUtc.Value - t.CreatedAtUtc).TotalDays).ToList();
				return new TeamPerformanceRowDto(team.Id, team.Key, team.Name, count, num, overdueTasks, (count == 0) ? 0.0 : ((double)num / (double)count), (list2.Count == 0) ? 0.0 : list2.Average());
			})
			orderby t.CompletionRate descending
			select t).ToList(), GeneratedAtUtc: DateTime.UtcNow);
	}

	private IQueryable<TaskItem> ApplyTaskFilters(Guid tenantId, ReportFilter filter)
	{
		IQueryable<TaskItem> queryable = context.TaskItems.AsNoTracking().Where((TaskItem t) => t.TenantId == tenantId);
		if (filter.TeamId.HasValue)
		{
			queryable = queryable.Where((TaskItem t) => t.TeamId == filter.TeamId.Value);
		}
		if (filter.ProjectId.HasValue)
		{
			queryable = queryable.Where((TaskItem t) => t.ProjectId == filter.ProjectId.Value);
		}
		if (filter.AssigneeId.HasValue)
		{
			queryable = queryable.Where((TaskItem t) => t.AssigneeId == filter.AssigneeId.Value);
		}
		if (filter.From.HasValue)
		{
			DateTime fromUtc = filter.From.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
			queryable = queryable.Where((TaskItem t) => t.CreatedAtUtc >= fromUtc);
		}
		if (filter.To.HasValue)
		{
			DateTime toUtc = filter.To.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
			queryable = queryable.Where((TaskItem t) => t.CreatedAtUtc <= toUtc);
		}
		return queryable;
	}

	private IQueryable<TaskRowProjection> SelectTaskRows(Guid tenantId, IQueryable<TaskItem> tasks)
	{
		return from t in tasks
			join team in context.Teams.AsNoTracking() on t.TeamId equals team.Id
			where team.TenantId == tenantId
			join state in context.WorkflowStates.AsNoTracking() on t.WorkflowStateId equals state.Id
			where state.TenantId == tenantId
			join project in context.Projects.AsNoTracking() on t.ProjectId equals project.Id into projects
			from project in projects.DefaultIfEmpty()
			select new TaskRowProjection(t.Id, string.Concat(team.Key + "-", t.Number), t.Title, team.Key, (project != null) ? project.Key : null, state.Name, t.Priority.ToString(), t.AssigneeId, t.DueDate, t.CreatedAtUtc, t.WorkflowStateId);
	}

	private async Task<HashSet<Guid>> GetCompletedStateIdSetAsync(Guid tenantId, CancellationToken ct)
	{
		return (await (from s in context.WorkflowStates.AsNoTracking()
			where s.TenantId == tenantId && (int)s.Type == 3
			select s.Id).ToListAsync(ct)).ToHashSet();
	}

	private static ReportTaskRowDto MapRow(TaskRowProjection row)
	{
		return new ReportTaskRowDto(row.TaskId, row.Identifier, row.Title, row.TeamKey, row.ProjectKey, row.Status, row.Priority, row.AssigneeId, row.DueDate, row.CreatedAtUtc);
	}
}
