using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Queries.ListLabels;

public sealed record ListLabelsQuery(PaginationParams Pagination) : IRequest<Result<PagedList<LabelDto>>>, IBaseRequest, IQuery;
