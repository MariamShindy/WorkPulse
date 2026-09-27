using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Collaboration.Dtos;
using WorkPulse.Application.Common.Pagination;

namespace WorkPulse.Application.Collaboration.Queries.ListTaskComments;

public sealed record ListTaskCommentsQuery(Guid TaskId, PaginationParams Pagination) : IRequest<Result<PagedList<TaskCommentDto>>>, IBaseRequest, IQuery;
