using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Reports.Dtos;

namespace WorkPulse.Infrastructure.Services.Read;

public sealed class ReportsReadService(ApplicationDbContext context) : IReportsReadService
{
	public async Task<TaskSummaryReportDto> GetTaskSummaryReportAsync(
		Guid tenantId,
		ReportFilter filter,
		CancellationToken cancellationToken)
	{
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
		HashSet<Guid> completedStateIds = await GetCompletedStateIdsAsync(tenantId, cancellationToken);

		IQueryable<TaskItem> filteredTasks = ApplyTaskFilters(tenantId, filter)
			.OrderByDescending(task => task.CreatedAtUtc)
			.Take(5000);

		List<TaskRowProjection> rows = await SelectTaskRows(tenantId, filteredTasks).ToListAsync(cancellationToken);

		return new TaskSummaryReportDto(
			DateTime.UtcNow,
			rows.Count,
			rows.Count(row => !completedStateIds.Contains(row.WorkflowStateId)),
			rows.Count(row => completedStateIds.Contains(row.WorkflowStateId)),
			rows.Count(row => row.DueDate.HasValue && row.DueDate < today && !completedStateIds.Contains(row.WorkflowStateId)),
			rows.Select(MapRow).ToList());
	}

	public async Task<OverdueTasksReportDto> GetOverdueTasksReportAsync(
		Guid tenantId,
		ReportFilter filter,
		CancellationToken cancellationToken)
	{
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
		HashSet<Guid> completedStateIds = await GetCompletedStateIdsAsync(tenantId, cancellationToken);

		IQueryable<TaskItem> overdueTasks = ApplyTaskFilters(tenantId, filter)
			.Where(task => task.DueDate != null && task.DueDate < today && !completedStateIds.Contains(task.WorkflowStateId))
			.OrderBy(task => task.DueDate)
			.Take(2000);

		List<TaskRowProjection> rows = await SelectTaskRows(tenantId, overdueTasks).ToListAsync(cancellationToken);
		return new OverdueTasksReportDto(DateTime.UtcNow, rows.Count, rows.Select(MapRow).ToList());
	}

	public async Task<TeamPerformanceReportDto> GetTeamPerformanceReportAsync(
		Guid tenantId,
		ReportFilter filter,
		CancellationToken cancellationToken)
	{
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
		HashSet<Guid> completedStateIds = await GetCompletedStateIdsAsync(tenantId, cancellationToken);

		IQueryable<Team> teamsQuery = context.Teams.AsNoTracking()
			.Where(team => team.TenantId == tenantId && !team.IsArchived);

		if (filter.TeamId.HasValue)
		{
			teamsQuery = teamsQuery.Where(team => team.Id == filter.TeamId.Value);
		}

		var teams = await teamsQuery
			.Select(team => new { team.Id, team.Key, team.Name })
			.ToListAsync(cancellationToken);

		IQueryable<TaskItem> tasksQuery = context.TaskItems.AsNoTracking()
			.Where(task => task.TenantId == tenantId);

		if (filter.TeamId.HasValue)
		{
			tasksQuery = tasksQuery.Where(task => task.TeamId == filter.TeamId.Value);
		}

		List<TaskMetricsRow> taskRows = await tasksQuery
			.Select(task => new TaskMetricsRow(
				task.TeamId,
				task.WorkflowStateId,
				task.DueDate,
				task.CreatedAtUtc,
				task.UpdatedAtUtc,
				task.StartedAtUtc,
				task.CompletedAtUtc))
			.ToListAsync(cancellationToken);

		List<TeamPerformanceRowDto> performanceRows = teams
			.Select(team => BuildTeamPerformanceRow(
				team.Id,
				team.Key,
				team.Name,
				taskRows,
				completedStateIds,
				today))
			.OrderByDescending(row => row.CompletionRate)
			.ToList();

		return new TeamPerformanceReportDto(Teams: performanceRows, GeneratedAtUtc: DateTime.UtcNow);
	}

	private static TeamPerformanceRowDto BuildTeamPerformanceRow(
		Guid teamId,
		string teamKey,
		string teamName,
		IReadOnlyList<TaskMetricsRow> taskRows,
		HashSet<Guid> completedStateIds,
		DateOnly today)
	{
		List<TaskMetricsRow> teamTasks = taskRows.Where(task => task.TeamId == teamId).ToList();
		int totalTasks = teamTasks.Count;
		int completedTasks = teamTasks.Count(task => completedStateIds.Contains(task.WorkflowStateId));
		int overdueTasks = teamTasks.Count(task =>
			task.DueDate.HasValue &&
			task.DueDate < today &&
			!completedStateIds.Contains(task.WorkflowStateId));

		List<double> cycleTimes = teamTasks
			.Where(task => completedStateIds.Contains(task.WorkflowStateId) && (task.CompletedAtUtc ?? task.UpdatedAtUtc).HasValue)
			.Select(task => ((task.CompletedAtUtc ?? task.UpdatedAtUtc)!.Value - (task.StartedAtUtc ?? task.CreatedAtUtc)).TotalDays)
			.ToList();

		double completionRate = totalTasks == 0 ? 0.0 : (double)completedTasks / totalTasks;
		double averageCycleTimeDays = cycleTimes.Count == 0 ? 0.0 : cycleTimes.Average();

		return new TeamPerformanceRowDto(
			teamId,
			teamKey,
			teamName,
			totalTasks,
			completedTasks,
			overdueTasks,
			completionRate,
			averageCycleTimeDays);
	}

	public async Task<IReadOnlyList<CycleTimeTaskRowDto>> GetCycleTimeTaskRowsAsync(
		Guid tenantId,
		ReportFilter filter,
		CancellationToken cancellationToken)
	{
		HashSet<Guid> completedStateIds = await GetCompletedStateIdsAsync(tenantId, cancellationToken);

		IQueryable<TaskItem> completedTasks = ApplyTaskFilters(tenantId, filter)
			.Where(task => completedStateIds.Contains(task.WorkflowStateId))
			.OrderByDescending(task => task.CreatedAtUtc)
			.Take(5000);

		var rows = await (
			from task in completedTasks
			join team in context.Teams.AsNoTracking() on task.TeamId equals team.Id
			where team.TenantId == tenantId
			select new
			{
				task.Id,
				Identifier = string.Concat(team.Key + "-", task.Number),
				task.Title,
				TeamKey = team.Key,
				task.AssigneeId,
				task.CreatedAtUtc,
				task.UpdatedAtUtc,
				task.StartedAtUtc,
				task.CompletedAtUtc
			}).ToListAsync(cancellationToken);

		return rows.Select(row =>
		{
			DateTime? effectiveCompleted = row.CompletedAtUtc ?? row.UpdatedAtUtc;
			DateTime effectiveStarted = row.StartedAtUtc ?? row.CreatedAtUtc;
			double? cycleTimeDays = effectiveCompleted.HasValue ? (effectiveCompleted.Value - effectiveStarted).TotalDays : null;
			double? leadTimeDays = effectiveCompleted.HasValue ? (effectiveCompleted.Value - row.CreatedAtUtc).TotalDays : null;
			return new CycleTimeTaskRowDto(
				row.Id,
				row.Identifier,
				row.Title,
				row.TeamKey,
				row.AssigneeId,
				row.StartedAtUtc,
				row.CompletedAtUtc,
				cycleTimeDays,
				leadTimeDays);
		}).ToList();
	}

	private IQueryable<TaskItem> ApplyTaskFilters(Guid tenantId, ReportFilter filter)
	{
		IQueryable<TaskItem> query = context.TaskItems.AsNoTracking()
			.Where(task => task.TenantId == tenantId);

		if (filter.TeamId.HasValue)
		{
			query = query.Where(task => task.TeamId == filter.TeamId.Value);
		}

		if (filter.ProjectId.HasValue)
		{
			query = query.Where(task => task.ProjectId == filter.ProjectId.Value);
		}

		if (filter.AssigneeId.HasValue)
		{
			query = query.Where(task => task.AssigneeId == filter.AssigneeId.Value);
		}

		if (filter.From.HasValue)
		{
			DateTime fromUtc = filter.From.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
			query = query.Where(task => task.CreatedAtUtc >= fromUtc);
		}

		if (filter.To.HasValue)
		{
			DateTime toUtc = filter.To.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
			query = query.Where(task => task.CreatedAtUtc <= toUtc);
		}

		return query;
	}

	private IQueryable<TaskRowProjection> SelectTaskRows(Guid tenantId, IQueryable<TaskItem> tasks)
	{
		return from task in tasks
			join team in context.Teams.AsNoTracking() on task.TeamId equals team.Id
			where team.TenantId == tenantId
			join state in context.WorkflowStates.AsNoTracking() on task.WorkflowStateId equals state.Id
			where state.TenantId == tenantId
			join project in context.Projects.AsNoTracking() on task.ProjectId equals project.Id into projects
			from project in projects.DefaultIfEmpty()
			select new TaskRowProjection(
				task.Id,
				string.Concat(team.Key + "-", task.Number),
				task.Title,
				team.Key,
				project != null ? project.Key : null,
				state.Name,
				task.Priority.ToString(),
				task.AssigneeId,
				task.DueDate,
				task.CreatedAtUtc,
				task.WorkflowStateId);
	}

	private async Task<HashSet<Guid>> GetCompletedStateIdsAsync(Guid tenantId, CancellationToken cancellationToken)
	{
		List<Guid> stateIds = await context.WorkflowStates.AsNoTracking()
			.Where(state => state.TenantId == tenantId && state.Type == WorkflowStateType.Completed)
			.Select(state => state.Id)
			.ToListAsync(cancellationToken);

		return stateIds.ToHashSet();
	}

	private static ReportTaskRowDto MapRow(TaskRowProjection row)
	{
		return new ReportTaskRowDto(
			row.TaskId,
			row.Identifier,
			row.Title,
			row.TeamKey,
			row.ProjectKey,
			row.Status,
			row.Priority,
			row.AssigneeId,
			row.DueDate,
			row.CreatedAtUtc);
	}
}
