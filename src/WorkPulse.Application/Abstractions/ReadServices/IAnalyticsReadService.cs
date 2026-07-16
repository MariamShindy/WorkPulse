using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WorkPulse.Application.Analytics.Dtos;

namespace WorkPulse.Application.Abstractions.ReadServices;

public interface IAnalyticsReadService
{
	Task<DashboardAnalyticsDto> GetDashboardAsync(Guid tenantId, Guid? teamId, DateOnly? from, DateOnly? to, CancellationToken ct);

	Task<IReadOnlyList<TeamVelocityPointDto>> GetTeamVelocityAsync(Guid tenantId, Guid? teamId, int weeks, CancellationToken ct);

	Task<IReadOnlyList<AssigneeWorkloadDto>> GetAssigneeWorkloadAsync(Guid tenantId, Guid? teamId, CancellationToken ct);

	Task<IReadOnlyList<ProjectProgressDto>> GetProjectProgressAsync(Guid tenantId, Guid? teamId, CancellationToken ct);
}
