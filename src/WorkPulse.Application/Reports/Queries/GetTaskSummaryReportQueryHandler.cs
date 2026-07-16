using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Reports.Dtos;

namespace WorkPulse.Application.Reports.Queries;

public sealed class GetTaskSummaryReportQueryHandler(IReportsReadService reports, ITenantContext tenantContext) : IRequestHandler<GetTaskSummaryReportQuery, Result<TaskSummaryReportDto>>
{
	public async Task<Result<TaskSummaryReportDto>> Handle(GetTaskSummaryReportQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		return await reports.GetTaskSummaryReportAsync(tenantContext.TenantId, request.Filter, ct);
	}
}
