using System;
using System.Threading;
using System.Threading.Tasks;
using WorkPulse.Application.Reports.Dtos;

namespace WorkPulse.Application.Abstractions.ReadServices;

public interface IReportsReadService
{
	Task<TaskSummaryReportDto> GetTaskSummaryReportAsync(Guid tenantId, ReportFilter filter, CancellationToken ct);

	Task<OverdueTasksReportDto> GetOverdueTasksReportAsync(Guid tenantId, ReportFilter filter, CancellationToken ct);

	Task<TeamPerformanceReportDto> GetTeamPerformanceReportAsync(Guid tenantId, ReportFilter filter, CancellationToken ct);
}
