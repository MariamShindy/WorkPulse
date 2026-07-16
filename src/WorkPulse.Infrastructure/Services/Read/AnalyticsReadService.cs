using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Analytics.Dtos;
using WorkPulse.Domain.Entities;
using WorkPulse.Domain.Enums;
using WorkPulse.Infrastructure.Persistence;

namespace WorkPulse.Infrastructure.Services.Read;

public sealed class AnalyticsReadService(ApplicationDbContext context) : IAnalyticsReadService
{
	public async Task<DashboardAnalyticsDto> GetDashboardAsync(Guid tenantId, Guid? teamId, DateOnly? from, DateOnly? to, CancellationToken ct)
	{
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
		DateOnly fromDate = from ?? today.AddDays(-30);
		DateOnly toDate = to.GetValueOrDefault(today);
		DateTime fromUtc = fromDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
		DateTime toUtc = toDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
		IQueryable<TaskItem> tasks = context.TaskItems.AsNoTracking().Where((TaskItem t) => t.TenantId == tenantId);
		if (teamId.HasValue)
		{
			tasks = tasks.Where((TaskItem t) => t.TeamId == ((Guid?)teamId).Value);
		}
		tasks = tasks.Where((TaskItem t) => t.CreatedAtUtc >= fromUtc && t.CreatedAtUtc <= toUtc);
		HashSet<Guid> completedSet = (await (from s in context.WorkflowStates.AsNoTracking()
			where s.TenantId == tenantId && (int)s.Type == 3
			select s.Id).ToListAsync(ct)).ToHashSet();
		int totalTasks = await tasks.CountAsync(ct);
		int openTasks = await tasks.CountAsync((TaskItem t) => !completedSet.Contains(t.WorkflowStateId), ct);
		int completedTasks = await tasks.CountAsync((TaskItem t) => completedSet.Contains(t.WorkflowStateId), ct);
		int overdueTasks = await tasks.CountAsync((TaskItem t) => t.DueDate != null && t.DueDate < today && !completedSet.Contains(t.WorkflowStateId), ct);
		int unassignedTasks = await tasks.CountAsync((TaskItem t) => t.AssigneeId == null, ct);
		List<StatusCountDto> tasksByStatus = await (from t in tasks
			join s in context.WorkflowStates.AsNoTracking() on t.WorkflowStateId equals s.Id
			group t by new { s.Name, s.Type } into g
			orderby g.Count() descending
			select new StatusCountDto(g.Key.Name, g.Key.Type.ToString(), g.Count())).ToListAsync(ct);
		List<PriorityCountDto> tasksByPriority = (await (from t in tasks
			group t by t.Priority into g
			select new
			{
				Key = g.Key,
				Count = g.Count()
			} into p
			orderby p.Count descending
			select p).ToListAsync(ct)).Select(p => new PriorityCountDto(p.Key.ToString(), p.Count)).ToList();
		List<DailyCountDto> tasksCreatedByDay = (await (from t in tasks
			group t by DateOnly.FromDateTime(t.CreatedAtUtc) into g
			select new
			{
				Date = g.Key,
				Count = g.Count()
			} into d
			orderby d.Date
			select d).ToListAsync(ct)).Select(d => new DailyCountDto(d.Date, d.Count)).ToList();
		List<DailyCountDto> tasksCompletedByDay = (await (from t in tasks
			where completedSet.Contains(t.WorkflowStateId) && t.UpdatedAtUtc != null
			group t by DateOnly.FromDateTime(t.UpdatedAtUtc.Value) into g
			select new
			{
				Date = g.Key,
				Count = g.Count()
			} into d
			orderby d.Date
			select d).ToListAsync(ct)).Select(d => new DailyCountDto(d.Date, d.Count)).ToList();
		var cycleSamples = await (from t in tasks
			where completedSet.Contains(t.WorkflowStateId) && t.UpdatedAtUtc != null
			select new { t.CreatedAtUtc, t.UpdatedAtUtc }).ToListAsync(ct);
		double averageCycleTimeDays = ((cycleSamples.Count == 0) ? 0.0 : cycleSamples.Average(t => (t.UpdatedAtUtc.Value - t.CreatedAtUtc).TotalDays));
		return new DashboardAnalyticsDto(totalTasks, openTasks, completedTasks, overdueTasks, unassignedTasks, tasksByStatus, tasksByPriority, tasksCreatedByDay, tasksCompletedByDay, averageCycleTimeDays);
	}

	public async Task<IReadOnlyList<TeamVelocityPointDto>> GetTeamVelocityAsync(Guid tenantId, Guid? teamId, int weeks, CancellationToken ct)
	{
		DateOnly start = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-7 * (weeks - 1));
		DateTime startUtc = start.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
		HashSet<Guid> completedSet = (await (from s in context.WorkflowStates.AsNoTracking()
			where s.TenantId == tenantId && (int)s.Type == 3
			select s.Id).ToListAsync(ct)).ToHashSet();
		IQueryable<TaskItem> taskQuery = from t in context.TaskItems.AsNoTracking()
			where t.TenantId == tenantId && t.CreatedAtUtc >= startUtc
			select t;
		if (teamId.HasValue)
		{
			taskQuery = taskQuery.Where((TaskItem t) => t.TeamId == ((Guid?)teamId).Value);
		}
		var taskDates = await taskQuery.Select((TaskItem t) => new
		{
			Created = DateOnly.FromDateTime(t.CreatedAtUtc),
			Completed = ((t.UpdatedAtUtc.HasValue && completedSet.Contains(t.WorkflowStateId)) ? ((DateOnly?)DateOnly.FromDateTime(t.UpdatedAtUtc.Value)) : ((DateOnly?)null))
		}).ToListAsync(ct);
		List<TeamVelocityPointDto> points = new List<TeamVelocityPointDto>();
		for (int i = 0; i < weeks; i++)
		{
			DateOnly weekStart = start.AddDays(i * 7);
			DateOnly weekEnd = weekStart.AddDays(6);
			points.Add(new TeamVelocityPointDto(weekStart, taskDates.Count(t => t.Created >= weekStart && t.Created <= weekEnd), taskDates.Count(t => t.Completed >= weekStart && t.Completed <= weekEnd)));
		}
		return points;
	}

	public async Task<IReadOnlyList<AssigneeWorkloadDto>> GetAssigneeWorkloadAsync(Guid tenantId, Guid? teamId, CancellationToken ct)
	{
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
		HashSet<Guid> completedSet = (await (from s in context.WorkflowStates.AsNoTracking()
			where s.TenantId == tenantId && (int)s.Type == 3
			select s.Id).ToListAsync(ct)).ToHashSet();
		IQueryable<TaskItem> tasks = from t in context.TaskItems.AsNoTracking()
			where t.TenantId == tenantId && t.AssigneeId != null
			select t;
		if (teamId.HasValue)
		{
			tasks = tasks.Where((TaskItem t) => t.TeamId == ((Guid?)teamId).Value);
		}
		return (from t in await tasks.Select((TaskItem t) => new { t.AssigneeId, t.DueDate, t.WorkflowStateId }).ToListAsync(ct)
			group t by t.AssigneeId.Value into g
			select new AssigneeWorkloadDto(g.Key, g.Count(t => !completedSet.Contains(t.WorkflowStateId)), g.Count(t => t.DueDate.HasValue && t.DueDate < today && !completedSet.Contains(t.WorkflowStateId)), g.Count(t => completedSet.Contains(t.WorkflowStateId))) into x
			orderby x.OpenTasks descending
			select x).ToList();
	}

	public async Task<IReadOnlyList<ProjectProgressDto>> GetProjectProgressAsync(Guid tenantId, Guid? teamId, CancellationToken ct)
	{
		HashSet<Guid> completedSet = (await (from s in context.WorkflowStates.AsNoTracking()
			where s.TenantId == tenantId && (int)s.Type == 3
			select s.Id).ToListAsync(ct)).ToHashSet();
		IQueryable<Project> projects = from p in context.Projects.AsNoTracking()
			where p.TenantId == tenantId && !p.IsArchived
			select p;
		if (teamId.HasValue)
		{
			projects = projects.Where((Project p) => p.TeamId == ((Guid?)teamId).Value);
		}
		var projectList = await projects.Select((Project p) => new { p.Id, p.Key, p.Name }).ToListAsync(ct);
		var taskRows = await (from t in context.TaskItems.AsNoTracking()
			where t.TenantId == tenantId && t.ProjectId != null
			select new
			{
				ProjectId = t.ProjectId.Value,
				WorkflowStateId = t.WorkflowStateId
			}).ToListAsync(ct);
		return (from p in projectList.Select(p =>
			{
				var list = taskRows.Where(t => t.ProjectId == p.Id).ToList();
				int count = list.Count;
				int num = list.Count(t => completedSet.Contains(t.WorkflowStateId));
				return new ProjectProgressDto(p.Id, p.Key, p.Name, count, num, (count == 0) ? 0.0 : ((double)num / (double)count));
			})
			orderby p.CompletionRate descending
			select p).ToList();
	}
}
