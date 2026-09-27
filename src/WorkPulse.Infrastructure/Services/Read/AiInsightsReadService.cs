using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.AiChat.Dtos;

namespace WorkPulse.Infrastructure.Services.Read;

/// <summary>
/// Tenant-scoped analytics tailored for the AI assistant's tools. All numbers
/// come straight from the database; assignee ids are resolved to names via the
/// identity service so the model can talk about people by name.
/// </summary>
public sealed class AiInsightsReadService(
    ApplicationDbContext context,
    IUserIdentityService identity) : IAiInsightsReadService
{
    /// <summary>Workflow-state ids that count as "done" (state Type == Completed == 3).</summary>
    private async Task<HashSet<Guid>> GetCompletedStatesAsync(Guid tenantId, CancellationToken ct)
    {
        List<Guid> ids = await context.WorkflowStates.AsNoTracking()
            .Where(s => s.TenantId == tenantId && s.Type == WorkflowStateType.Completed)
            .Select(s => s.Id)
            .ToListAsync(ct);
        return ids.ToHashSet();
    }

    public async Task<IReadOnlyList<MemberWorkloadDto>> GetMemberWorkloadAsync(Guid tenantId, CancellationToken ct)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        HashSet<Guid> completed = await GetCompletedStatesAsync(tenantId, ct);

        var rows = await context.TaskItems.AsNoTracking()
            .Where(t => t.TenantId == tenantId && t.AssigneeId != null)
            .Select(t => new { t.AssigneeId, t.DueDate, t.WorkflowStateId })
            .ToListAsync(ct);

        var grouped = rows
            .GroupBy(t => t.AssigneeId!.Value)
            .Select(g => new
            {
                UserId = g.Key,
                Open = g.Count(t => !completed.Contains(t.WorkflowStateId)),
                Overdue = g.Count(t => t.DueDate.HasValue && t.DueDate < today && !completed.Contains(t.WorkflowStateId)),
                Completed = g.Count(t => completed.Contains(t.WorkflowStateId)),
                Total = g.Count()
            })
            .OrderByDescending(x => x.Open)
            .ToList();

        IReadOnlyDictionary<Guid, UserIdentityDto> users =
            await identity.GetByIdsAsync(grouped.Select(x => x.UserId), ct);

        return grouped.Select(x =>
        {
            users.TryGetValue(x.UserId, out UserIdentityDto? user);
            string name = user is null ? "Unknown user" : $"{user.FirstName} {user.LastName}".Trim();
            return new MemberWorkloadDto(
                x.UserId,
                string.IsNullOrWhiteSpace(name) ? "Unknown user" : name,
                user?.Email ?? "",
                x.Open, x.Overdue, x.Completed, x.Total);
        }).ToList();
    }

    public async Task<IReadOnlyList<MonthlyKpiDto>> GetMonthlyKpisAsync(Guid tenantId, int months, CancellationToken ct)
    {
        if (months < 1) months = 1;
        if (months > 24) months = 24;

        DateTime nowUtc = DateTime.UtcNow;
        // First day of the earliest month in range.
        DateTime rangeStart = new DateTime(nowUtc.Year, nowUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc)
            .AddMonths(-(months - 1));

        HashSet<Guid> completed = await GetCompletedStatesAsync(tenantId, ct);

        var rows = await context.TaskItems.AsNoTracking()
            .Where(t => t.TenantId == tenantId && t.CreatedAtUtc >= rangeStart)
            .Select(t => new { t.CreatedAtUtc, t.UpdatedAtUtc, t.WorkflowStateId, t.DueDate })
            .ToListAsync(ct);

        DateOnly today = DateOnly.FromDateTime(nowUtc);
        var result = new List<MonthlyKpiDto>();

        for (int i = 0; i < months; i++)
        {
            DateTime monthStart = rangeStart.AddMonths(i);
            DateTime monthEnd = monthStart.AddMonths(1);
            string label = monthStart.ToString("yyyy-MM");

            var created = rows.Where(r => r.CreatedAtUtc >= monthStart && r.CreatedAtUtc < monthEnd).ToList();

            var completedThisMonth = rows.Where(r =>
                r.UpdatedAtUtc.HasValue &&
                completed.Contains(r.WorkflowStateId) &&
                r.UpdatedAtUtc.Value >= monthStart && r.UpdatedAtUtc.Value < monthEnd).ToList();

            int overdue = created.Count(r =>
                r.DueDate.HasValue && r.DueDate < today && !completed.Contains(r.WorkflowStateId));

            double avgCycle = completedThisMonth.Count == 0
                ? 0d
                : completedThisMonth.Average(r => (r.UpdatedAtUtc!.Value - r.CreatedAtUtc).TotalDays);

            result.Add(new MonthlyKpiDto(
                label,
                created.Count,
                completedThisMonth.Count,
                overdue,
                Math.Round(avgCycle, 1)));
        }

        return result;
    }

    public async Task<IReadOnlyList<OverdueTaskDto>> GetOverdueTasksAsync(Guid tenantId, CancellationToken ct)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        HashSet<Guid> completed = await GetCompletedStatesAsync(tenantId, ct);

        var rows = await (
            from t in context.TaskItems.AsNoTracking()
            join team in context.Teams.AsNoTracking() on t.TeamId equals team.Id
            join state in context.WorkflowStates.AsNoTracking() on t.WorkflowStateId equals state.Id
            where t.TenantId == tenantId
                && t.DueDate != null
                && t.DueDate < today
            select new
            {
                Identifier = team.Key + "-" + t.Number,
                t.Title,
                t.AssigneeId,
                DueDate = t.DueDate!.Value,
                t.Priority,
                StateName = state.Name,
                t.WorkflowStateId
            }).ToListAsync(ct);

        // Exclude tasks already completed.
        rows = rows.Where(r => !completed.Contains(r.WorkflowStateId)).ToList();

        var assigneeIds = rows.Where(r => r.AssigneeId.HasValue).Select(r => r.AssigneeId!.Value).Distinct();
        IReadOnlyDictionary<Guid, UserIdentityDto> users = await identity.GetByIdsAsync(assigneeIds, ct);

        return rows
            .OrderByDescending(r => today.DayNumber - r.DueDate.DayNumber)
            .Select(r =>
            {
                string assignee = "Unassigned";
                if (r.AssigneeId.HasValue && users.TryGetValue(r.AssigneeId.Value, out UserIdentityDto? u))
                {
                    string name = $"{u.FirstName} {u.LastName}".Trim();
                    assignee = string.IsNullOrWhiteSpace(name) ? u.Email : name;
                }
                return new OverdueTaskDto(
                    r.Identifier,
                    r.Title,
                    assignee,
                    r.DueDate,
                    today.DayNumber - r.DueDate.DayNumber,
                    r.Priority.ToString(),
                    r.StateName);
            })
            .ToList();
    }

    public async Task<WorkspaceSummaryDto> GetWorkspaceSummaryAsync(Guid tenantId, CancellationToken ct)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        HashSet<Guid> completed = await GetCompletedStatesAsync(tenantId, ct);

        var taskRows = await context.TaskItems.AsNoTracking()
            .Where(t => t.TenantId == tenantId)
            .Select(t => new { t.AssigneeId, t.DueDate, t.WorkflowStateId })
            .ToListAsync(ct);

        int total = taskRows.Count;
        int completedCount = taskRows.Count(t => completed.Contains(t.WorkflowStateId));
        int open = total - completedCount;
        int overdue = taskRows.Count(t => t.DueDate.HasValue && t.DueDate < today && !completed.Contains(t.WorkflowStateId));
        int unassigned = taskRows.Count(t => t.AssigneeId == null);

        int teamCount = await context.Teams.AsNoTracking().CountAsync(t => t.TenantId == tenantId && !t.IsArchived, ct);
        int projectCount = await context.Projects.AsNoTracking().CountAsync(p => p.TenantId == tenantId && !p.IsArchived, ct);
        int memberCount = taskRows.Where(t => t.AssigneeId.HasValue).Select(t => t.AssigneeId!.Value).Distinct().Count();

        return new WorkspaceSummaryDto(total, open, completedCount, overdue, unassigned, memberCount, teamCount, projectCount);
    }
}
