using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Queries.ListSprints;

public sealed record ListSprintsQuery(PaginationParams Pagination, Guid? TeamId = null, SprintStatus? Status = null) : IRequest<Result<PagedList<SprintDto>>>, IBaseRequest, IQuery;
