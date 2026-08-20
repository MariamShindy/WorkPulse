using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Collaboration.Dtos;
using WorkPulse.Application.Common.Pagination;

namespace WorkPulse.Application.Collaboration.Queries.ListTaskActivity;

public sealed record ListTaskActivityQuery(Guid TaskId, PaginationParams Pagination) : IRequest<Result<PagedList<TaskActivityDto>>>, IBaseRequest, IQuery;
