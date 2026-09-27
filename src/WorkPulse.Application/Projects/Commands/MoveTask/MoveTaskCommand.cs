using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Commands.MoveTask;

public sealed record MoveTaskCommand(Guid TaskId, Guid WorkflowStateId, int? SortOrder) : IRequest<Result<TaskItemDto>>, IBaseRequest, ICommand;
