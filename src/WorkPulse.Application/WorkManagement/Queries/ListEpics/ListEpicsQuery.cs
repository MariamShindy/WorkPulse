using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.WorkManagement.Dtos;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.WorkManagement.Queries.ListEpics;

public sealed record ListEpicsQuery(PaginationParams Pagination, Guid? TeamId = null, EpicStatus? Status = null) : IRequest<Result<PagedList<EpicDto>>>, IBaseRequest, IQuery;
