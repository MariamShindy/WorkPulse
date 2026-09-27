using WorkPulse.Application.AiChat.Dtos;

namespace WorkPulse.Application.Abstractions.ReadServices;

/// <summary>
/// Grounded, tenant-scoped data source for the AI assistant's tools.
/// Each method maps 1:1 to a function the model can call, so answers are
/// backed by real database numbers rather than hallucinated ones.
/// </summary>
public interface IAiInsightsReadService
{
    Task<IReadOnlyList<MemberWorkloadDto>> GetMemberWorkloadAsync(Guid tenantId, CancellationToken ct);

    Task<IReadOnlyList<MonthlyKpiDto>> GetMonthlyKpisAsync(Guid tenantId, int months, CancellationToken ct);

    Task<IReadOnlyList<OverdueTaskDto>> GetOverdueTasksAsync(Guid tenantId, CancellationToken ct);

    Task<WorkspaceSummaryDto> GetWorkspaceSummaryAsync(Guid tenantId, CancellationToken ct);
}
