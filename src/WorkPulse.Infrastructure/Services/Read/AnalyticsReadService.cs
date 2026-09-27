using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Analytics;
using WorkPulse.Application.Analytics.Dtos;

namespace WorkPulse.Infrastructure.Services.Read;

public sealed class AnalyticsReadService(ApplicationDbContext context) : IAnalyticsReadService
{
	public async Task<DashboardAnalyticsDto> GetDashboardAsync(
		Guid tenantId,
		Guid? teamId,
		DateOnly? from,
		DateOnly? to,
		CancellationToken cancellationToken)
	{
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
		IQueryable<TaskItem> tasks = BuildTaskQuery(tenantId, teamId, from, to);
		HashSet<Guid> completedStateIds = await GetCompletedStateIdsAsync(tenantId, cancellationToken);

		var counts = await tasks
			.GroupBy(_ => 1)
			.Select(group => new
			{
				TotalTasks = group.Count(),
				OpenTasks = group.Count(task => !completedStateIds.Contains(task.WorkflowStateId)),
				CompletedTasks = group.Count(task => completedStateIds.Contains(task.WorkflowStateId)),
				OverdueTasks = group.Count(task =>
					task.DueDate != null &&
					task.DueDate < today &&
					!completedStateIds.Contains(task.WorkflowStateId)),
				UnassignedTasks = group.Count(task => task.AssigneeId == null)
			})
			.FirstOrDefaultAsync(cancellationToken);

		int totalTasks = counts?.TotalTasks ?? 0;
		int openTasks = counts?.OpenTasks ?? 0;
		int completedTasks = counts?.CompletedTasks ?? 0;
		int overdueTasks = counts?.OverdueTasks ?? 0;
		int unassignedTasks = counts?.UnassignedTasks ?? 0;

		List<StatusCountDto> tasksByStatus = await GetTasksByStatusAsync(tasks, cancellationToken);
		List<PriorityCountDto> tasksByPriority = await GetTasksByPriorityAsync(tasks, cancellationToken);
		List<DailyCountDto> tasksCreatedByDay = await GetTasksCreatedByDayAsync(tasks, cancellationToken);
		List<DailyCountDto> tasksCompletedByDay = await GetTasksCompletedByDayAsync(tasks, completedStateIds, cancellationToken);
		double averageCycleTimeDays = await GetAverageCycleTimeDaysAsync(tasks, completedStateIds, cancellationToken);

		return new DashboardAnalyticsDto(
			totalTasks,
			openTasks,
			completedTasks,
			overdueTasks,
			unassignedTasks,
			tasksByStatus,
			tasksByPriority,
			tasksCreatedByDay,
			tasksCompletedByDay,
			averageCycleTimeDays);
	}

	public async Task<IReadOnlyList<TeamVelocityPointDto>> GetTeamVelocityAsync(
		Guid tenantId,
		Guid? teamId,
		int weeks,
		CancellationToken cancellationToken)
	{
		DateOnly startDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-7 * (weeks - 1));
		DateTime startUtc = startDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
		HashSet<Guid> completedStateIds = await GetCompletedStateIdsAsync(tenantId, cancellationToken);

		IQueryable<TaskItem> taskQuery = context.TaskItems.AsNoTracking()
			.Where(task => task.TenantId == tenantId && task.CreatedAtUtc >= startUtc);

		if (teamId.HasValue)
		{
			taskQuery = taskQuery.Where(task => task.TeamId == teamId.Value);
		}

		var taskDates = await taskQuery.Select(task => new
		{
			Created = DateOnly.FromDateTime(task.CreatedAtUtc),
			Completed = task.UpdatedAtUtc.HasValue && completedStateIds.Contains(task.WorkflowStateId)
				? (DateOnly?)DateOnly.FromDateTime(task.UpdatedAtUtc.Value)
				: null
		}).ToListAsync(cancellationToken);

		List<TeamVelocityPointDto> velocityPoints = [];
		for (int weekIndex = 0; weekIndex < weeks; weekIndex++)
		{
			DateOnly weekStart = startDate.AddDays(weekIndex * 7);
			DateOnly weekEnd = weekStart.AddDays(6);
			velocityPoints.Add(new TeamVelocityPointDto(
				weekStart,
				taskDates.Count(task => task.Created >= weekStart && task.Created <= weekEnd),
				taskDates.Count(task => task.Completed >= weekStart && task.Completed <= weekEnd)));
		}

		return velocityPoints;
	}

	public async Task<IReadOnlyList<AssigneeWorkloadDto>> GetAssigneeWorkloadAsync(
		Guid tenantId,
		Guid? teamId,
		CancellationToken cancellationToken)
	{
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
		HashSet<Guid> completedStateIds = await GetCompletedStateIdsAsync(tenantId, cancellationToken);

		IQueryable<TaskItem> tasks = context.TaskItems.AsNoTracking()
			.Where(task => task.TenantId == tenantId && task.AssigneeId != null);

		if (teamId.HasValue)
		{
			tasks = tasks.Where(task => task.TeamId == teamId.Value);
		}

		var taskRows = await tasks
			.Select(task => new { task.AssigneeId, task.DueDate, task.WorkflowStateId })
			.ToListAsync(cancellationToken);

		return taskRows
			.GroupBy(task => task.AssigneeId!.Value)
			.Select(group => new AssigneeWorkloadDto(
				group.Key,
				group.Count(task => !completedStateIds.Contains(task.WorkflowStateId)),
				group.Count(task => task.DueDate.HasValue && task.DueDate < today && !completedStateIds.Contains(task.WorkflowStateId)),
				group.Count(task => completedStateIds.Contains(task.WorkflowStateId))))
			.OrderByDescending(workload => workload.OpenTasks)
			.ToList();
	}

	public async Task<IReadOnlyList<ProjectProgressDto>> GetProjectProgressAsync(
		Guid tenantId,
		Guid? teamId,
		CancellationToken cancellationToken)
	{
		HashSet<Guid> completedStateIds = await GetCompletedStateIdsAsync(tenantId, cancellationToken);

		IQueryable<Project> projects = context.Projects.AsNoTracking()
			.Where(project => project.TenantId == tenantId && !project.IsArchived);

		if (teamId.HasValue)
		{
			projects = projects.Where(project => project.TeamId == teamId.Value);
		}

		var projectList = await projects
			.Select(project => new { project.Id, project.Key, project.Name })
			.ToListAsync(cancellationToken);

		var taskRows = await context.TaskItems.AsNoTracking()
			.Where(task => task.TenantId == tenantId && task.ProjectId != null)
			.Select(task => new
			{
				ProjectId = task.ProjectId!.Value,
				task.WorkflowStateId
			})
			.ToListAsync(cancellationToken);

		ILookup<Guid, Guid> tasksByProject = taskRows.ToLookup(task => task.ProjectId, task => task.WorkflowStateId);

		return projectList
			.Select(project =>
			{
				IReadOnlyList<Guid> projectStateIds = tasksByProject[project.Id].ToList();
				int totalCount = projectStateIds.Count;
				int completedCount = projectStateIds.Count(stateId => completedStateIds.Contains(stateId));
				double completionRate = totalCount == 0 ? 0.0 : (double)completedCount / totalCount;
				return new ProjectProgressDto(project.Id, project.Key, project.Name, totalCount, completedCount, completionRate);
			})
			.OrderByDescending(progress => progress.CompletionRate)
			.ToList();
	}

	public async Task<CycleTimeAnalyticsDto> GetCycleTimeAnalyticsAsync(
		Guid tenantId,
		Guid? teamId,
		DateOnly? from,
		DateOnly? to,
		CancellationToken cancellationToken)
	{
		HashSet<Guid> completedStateIds = await GetCompletedStateIdsAsync(tenantId, cancellationToken);
		IQueryable<TaskItem> tasks = BuildTaskQuery(tenantId, teamId, from, to)
			.Where(task => completedStateIds.Contains(task.WorkflowStateId));

		var samples = await tasks
			.Select(task => new { task.CreatedAtUtc, task.UpdatedAtUtc, task.StartedAtUtc, task.CompletedAtUtc })
			.ToListAsync(cancellationToken);

		List<double> cycleTimes = [];
		List<double> leadTimes = [];
		foreach (var sample in samples)
		{
			DateTime? effectiveCompleted = sample.CompletedAtUtc ?? sample.UpdatedAtUtc;
			if (effectiveCompleted is null)
			{
				continue;
			}

			DateTime effectiveStarted = sample.StartedAtUtc ?? sample.CreatedAtUtc;
			cycleTimes.Add((effectiveCompleted.Value - effectiveStarted).TotalDays);
			leadTimes.Add((effectiveCompleted.Value - sample.CreatedAtUtc).TotalDays);
		}

		cycleTimes.Sort();

		return new CycleTimeAnalyticsDto(
			cycleTimes.Count,
			cycleTimes.Count == 0 ? 0.0 : cycleTimes.Average(),
			Percentile(cycleTimes, 50.0),
			Percentile(cycleTimes, 85.0),
			leadTimes.Count == 0 ? 0.0 : leadTimes.Average(),
			BuildCycleTimeHistogram(cycleTimes));
	}

	public async Task<IReadOnlyList<ThroughputPointDto>> GetThroughputAsync(
		Guid tenantId,
		Guid? teamId,
		AnalyticsGranularity granularity,
		int periods,
		CancellationToken cancellationToken)
	{
		HashSet<Guid> completedStateIds = await GetCompletedStateIdsAsync(tenantId, cancellationToken);
		int periodDays = granularity == AnalyticsGranularity.Week ? 7 : 1;
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
		DateOnly windowStart = today.AddDays(-(periodDays * periods) + 1);
		DateTime windowStartUtc = windowStart.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

		IQueryable<TaskItem> tasks = context.TaskItems.AsNoTracking()
			.Where(task => task.TenantId == tenantId);

		if (teamId.HasValue)
		{
			tasks = tasks.Where(task => task.TeamId == teamId.Value);
		}

		var completions = await tasks
			.Where(task => completedStateIds.Contains(task.WorkflowStateId) &&
				(task.CompletedAtUtc ?? task.UpdatedAtUtc) >= windowStartUtc)
			.Select(task => new
			{
				CompletedAt = (task.CompletedAtUtc ?? task.UpdatedAtUtc)!.Value,
				task.StoryPoints
			})
			.ToListAsync(cancellationToken);

		List<ThroughputPointDto> points = [];
		for (int i = 0; i < periods; i++)
		{
			DateOnly periodStart = windowStart.AddDays(i * periodDays);
			DateOnly periodEnd = periodStart.AddDays(periodDays - 1);
			var periodCompletions = completions
				.Where(completion =>
					DateOnly.FromDateTime(completion.CompletedAt) >= periodStart &&
					DateOnly.FromDateTime(completion.CompletedAt) <= periodEnd)
				.ToList();

			points.Add(new ThroughputPointDto(
				periodStart,
				periodCompletions.Count,
				periodCompletions.Sum(completion => completion.StoryPoints ?? 0)));
		}

		return points;
	}

	public async Task<SprintBurndownDto?> GetSprintBurndownAsync(
		Guid tenantId,
		Guid sprintId,
		CancellationToken cancellationToken)
	{
		Sprint? sprint = await context.Sprints.AsNoTracking()
			.FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Id == sprintId, cancellationToken);
		if (sprint is null)
		{
			return null;
		}

		HashSet<Guid> completedStateIds = await GetCompletedStateIdsAsync(tenantId, cancellationToken);

		var taskRows = await context.TaskItems.AsNoTracking()
			.Where(task => task.TenantId == tenantId && task.SprintId == sprintId)
			.Select(task => new
			{
				task.StoryPoints,
				CompletedAt = task.CompletedAtUtc ?? (completedStateIds.Contains(task.WorkflowStateId) ? task.UpdatedAtUtc : null)
			})
			.ToListAsync(cancellationToken);

		bool usePoints = taskRows.Any(row => row.StoryPoints.HasValue && row.StoryPoints.Value > 0);
		string unit = usePoints ? "points" : "count";
		double UnitValue(int? storyPoints) => usePoints ? (storyPoints ?? 0) : 1.0;

		double totalScope = taskRows.Sum(row => UnitValue(row.StoryPoints));
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
		int totalDays = sprint.EndDate.DayNumber - sprint.StartDate.DayNumber;

		List<BurndownPointDto> points = [];
		for (DateOnly day = sprint.StartDate; day <= sprint.EndDate; day = day.AddDays(1))
		{
			double idealRemaining = totalDays <= 0
				? 0.0
				: Math.Max(0.0, totalScope * (1.0 - ((double)(day.DayNumber - sprint.StartDate.DayNumber) / totalDays)));

			DateOnly effectiveDay = day > today ? today : day;
			double completedCumulative = taskRows
				.Where(row => row.CompletedAt.HasValue && DateOnly.FromDateTime(row.CompletedAt.Value) <= effectiveDay)
				.Sum(row => UnitValue(row.StoryPoints));

			double remaining = Math.Max(0.0, totalScope - completedCumulative);
			points.Add(new BurndownPointDto(day, remaining, idealRemaining, completedCumulative));
		}

		return new SprintBurndownDto(sprint.Id, sprint.Name, sprint.StartDate, sprint.EndDate, unit, totalScope, points);
	}

	private static readonly (double UpperBoundDays, string Label)[] CycleTimeBuckets =
	[
		(1, "0-1d"),
		(2, "1-2d"),
		(3, "2-3d"),
		(5, "3-5d"),
		(8, "5-8d"),
		(13, "8-13d"),
		(21, "13-21d"),
		(34, "21-34d"),
		(double.PositiveInfinity, "34d+")
	];

	private static IReadOnlyList<CycleTimeBucketDto> BuildCycleTimeHistogram(IReadOnlyList<double> cycleTimeDays)
	{
		int[] counts = new int[CycleTimeBuckets.Length];
		foreach (double days in cycleTimeDays)
		{
			for (int i = 0; i < CycleTimeBuckets.Length; i++)
			{
				if (days <= CycleTimeBuckets[i].UpperBoundDays)
				{
					counts[i]++;
					break;
				}
			}
		}

		return CycleTimeBuckets
			.Select((bucket, index) => new CycleTimeBucketDto(bucket.Label, counts[index]))
			.ToList();
	}

	private static double Percentile(IReadOnlyList<double> sortedValues, double percentile)
	{
		if (sortedValues.Count == 0)
		{
			return 0.0;
		}

		double rank = percentile / 100.0 * (sortedValues.Count - 1);
		int lower = (int)Math.Floor(rank);
		int upper = (int)Math.Ceiling(rank);
		if (lower == upper)
		{
			return sortedValues[lower];
		}

		double weight = rank - lower;
		return sortedValues[lower] + (weight * (sortedValues[upper] - sortedValues[lower]));
	}

	private IQueryable<TaskItem> BuildTaskQuery(Guid tenantId, Guid? teamId, DateOnly? from, DateOnly? to)
	{
		// Only apply a created-at window when the caller asks for one.
		// Default (no from/to) = all tasks in the tenant — otherwise older seeded work shows as zeros.
		DateTime? fromUtc = from?.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
		DateTime? toUtc = to?.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

		IQueryable<TaskItem> tasks = context.TaskItems.AsNoTracking()
			.Where(task => task.TenantId == tenantId);

		if (teamId.HasValue)
		{
			tasks = tasks.Where(task => task.TeamId == teamId.Value);
		}

		if (fromUtc.HasValue)
		{
			tasks = tasks.Where(task => task.CreatedAtUtc >= fromUtc.Value);
		}

		if (toUtc.HasValue)
		{
			tasks = tasks.Where(task => task.CreatedAtUtc <= toUtc.Value);
		}

		return tasks;
	}

	private async Task<HashSet<Guid>> GetCompletedStateIdsAsync(Guid tenantId, CancellationToken cancellationToken)
	{
		List<Guid> stateIds = await context.WorkflowStates.AsNoTracking()
			.Where(state => state.TenantId == tenantId && state.Type == WorkflowStateType.Completed)
			.Select(state => state.Id)
			.ToListAsync(cancellationToken);

		return stateIds.ToHashSet();
	}

	private async Task<List<StatusCountDto>> GetTasksByStatusAsync(
		IQueryable<TaskItem> tasks,
		CancellationToken cancellationToken)
	{
		return await (
			from task in tasks
			join state in context.WorkflowStates.AsNoTracking() on task.WorkflowStateId equals state.Id
			group task by new { state.Name, state.Type } into statusGroup
			orderby statusGroup.Count() descending
			select new StatusCountDto(statusGroup.Key.Name, statusGroup.Key.Type.ToString(), statusGroup.Count())
		).ToListAsync(cancellationToken);
	}

	private static async Task<List<PriorityCountDto>> GetTasksByPriorityAsync(
		IQueryable<TaskItem> tasks,
		CancellationToken cancellationToken)
	{
		var priorityGroups = await (
			from task in tasks
			group task by task.Priority into priorityGroup
			select new
			{
				Priority = priorityGroup.Key,
				Count = priorityGroup.Count()
			}
			into grouped
			orderby grouped.Count descending
			select grouped).ToListAsync(cancellationToken);

		return priorityGroups
			.Select(group => new PriorityCountDto(group.Priority.ToString(), group.Count))
			.ToList();
	}

	private static async Task<List<DailyCountDto>> GetTasksCreatedByDayAsync(
		IQueryable<TaskItem> tasks,
		CancellationToken cancellationToken)
	{
		var dailyGroups = await (
			from task in tasks
			group task by DateOnly.FromDateTime(task.CreatedAtUtc) into dayGroup
			select new
			{
				Date = dayGroup.Key,
				Count = dayGroup.Count()
			}
			into grouped
			orderby grouped.Date
			select grouped).ToListAsync(cancellationToken);

		return dailyGroups
			.Select(group => new DailyCountDto(group.Date, group.Count))
			.ToList();
	}

	private static async Task<List<DailyCountDto>> GetTasksCompletedByDayAsync(
		IQueryable<TaskItem> tasks,
		HashSet<Guid> completedStateIds,
		CancellationToken cancellationToken)
	{
		var dailyGroups = await (
			from task in tasks
			where completedStateIds.Contains(task.WorkflowStateId) && task.UpdatedAtUtc != null
			group task by DateOnly.FromDateTime(task.UpdatedAtUtc!.Value) into dayGroup
			select new
			{
				Date = dayGroup.Key,
				Count = dayGroup.Count()
			}
			into grouped
			orderby grouped.Date
			select grouped).ToListAsync(cancellationToken);

		return dailyGroups
			.Select(group => new DailyCountDto(group.Date, group.Count))
			.ToList();
	}

	private static async Task<double> GetAverageCycleTimeDaysAsync(
		IQueryable<TaskItem> tasks,
		HashSet<Guid> completedStateIds,
		CancellationToken cancellationToken)
	{
		var cycleSamples = await (
			from task in tasks
			where completedStateIds.Contains(task.WorkflowStateId)
			select new { task.CreatedAtUtc, task.UpdatedAtUtc, task.StartedAtUtc, task.CompletedAtUtc }).ToListAsync(cancellationToken);

		List<double> cycleTimes = [];
		foreach (var sample in cycleSamples)
		{
			DateTime? effectiveCompleted = sample.CompletedAtUtc ?? sample.UpdatedAtUtc;
			if (effectiveCompleted is null)
			{
				continue;
			}

			DateTime effectiveStarted = sample.StartedAtUtc ?? sample.CreatedAtUtc;
			cycleTimes.Add((effectiveCompleted.Value - effectiveStarted).TotalDays);
		}

		return cycleTimes.Count == 0 ? 0.0 : cycleTimes.Average();
	}
}
