using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.WorkManagement.Dtos;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.WorkManagement.Queries.ListSprints;

public sealed record ListSprintsQuery(PaginationParams Pagination, Guid? TeamId = null, SprintStatus? Status = null) : IRequest<Result<PagedList<SprintDto>>>, IBaseRequest, IQuery;
