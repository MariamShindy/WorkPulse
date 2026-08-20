using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Reports.Dtos;

namespace WorkPulse.Application.Reports.Queries;

public sealed record GetTaskSummaryReportQuery(ReportFilter Filter) : IRequest<Result<TaskSummaryReportDto>>, IBaseRequest, IQuery;
