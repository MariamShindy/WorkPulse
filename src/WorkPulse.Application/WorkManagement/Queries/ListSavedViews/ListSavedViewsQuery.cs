using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.WorkManagement.Dtos;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.WorkManagement.Queries.ListSavedViews;

public sealed record ListSavedViewsQuery(PaginationParams Pagination, SavedViewEntityType? EntityType = null) : IRequest<Result<PagedList<SavedViewDto>>>, IBaseRequest, IQuery;
