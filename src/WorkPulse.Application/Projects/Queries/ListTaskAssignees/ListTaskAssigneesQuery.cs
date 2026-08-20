using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Queries.ListTaskAssignees;

public sealed record ListTaskAssigneesQuery(Guid TaskId) : IRequest<Result<IReadOnlyList<TaskAssigneeDto>>>, IBaseRequest, IQuery;
