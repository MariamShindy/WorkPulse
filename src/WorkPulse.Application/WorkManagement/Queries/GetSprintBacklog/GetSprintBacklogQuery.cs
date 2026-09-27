using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Queries.GetSprintBacklog;

public sealed record GetSprintBacklogQuery(Guid TeamId, Guid? SprintId, PaginationParams Pagination) : IRequest<Result<PagedList<TaskItemDto>>>, IBaseRequest, IQuery;
