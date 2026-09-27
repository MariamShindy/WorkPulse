using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Queries.ListTaskDependencies;

public sealed record ListTaskDependenciesQuery(Guid TaskId) : IRequest<Result<IReadOnlyList<TaskDependencyDto>>>, IBaseRequest, IQuery;
