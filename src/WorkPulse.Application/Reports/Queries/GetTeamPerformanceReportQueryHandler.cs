using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
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
