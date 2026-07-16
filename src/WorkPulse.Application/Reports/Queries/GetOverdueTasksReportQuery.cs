using MediatR;
using WorkPulse.Application.Abstractions.ReadServices;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Reports.Dtos;

namespace WorkPulse.Application.Reports.Queries;

public sealed record GetOverdueTasksReportQuery(ReportFilter Filter) : IRequest<Result<OverdueTasksReportDto>>, IBaseRequest, IQuery;
