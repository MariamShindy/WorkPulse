using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Collaboration.Dtos;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Collaboration.Queries.ListTaskActivity;

public sealed record ListTaskActivityQuery(Guid TaskId, PaginationParams Pagination) : IRequest<Result<PagedList<TaskActivityDto>>>, IBaseRequest, IQuery;
