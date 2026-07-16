using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Queries.ListWorkLogs;

public sealed record ListWorkLogsQuery(Guid TaskId, PaginationParams Pagination) : IRequest<Result<PagedList<WorkLogDto>>>, IBaseRequest, IQuery;
