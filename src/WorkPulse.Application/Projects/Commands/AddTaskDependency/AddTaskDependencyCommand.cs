using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Commands.AddTaskDependency;

public sealed record AddTaskDependencyCommand(Guid TaskId, Guid DependsOnTaskId, TaskDependencyType Type) : IRequest<Result<TaskDependencyDto>>, IBaseRequest, ICommand;
