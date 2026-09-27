using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Reports.Dtos;

namespace WorkPulse.Application.Reports.Queries;

public sealed class GetTeamPerformanceReportQueryHandler(IReportsReadService reports, ITenantContext tenantContext) : IRequestHandler<GetTeamPerformanceReportQuery, Result<TeamPerformanceReportDto>>
{
	public async Task<Result<TeamPerformanceReportDto>> Handle(GetTeamPerformanceReportQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		return await reports.GetTeamPerformanceReportAsync(tenantContext.TenantId, request.Filter, ct);
	}
}
