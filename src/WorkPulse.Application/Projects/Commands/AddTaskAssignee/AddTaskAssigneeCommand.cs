using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Projects.Commands.AddTaskAssignee;

public sealed record AddTaskAssigneeCommand(Guid TaskId, Guid UserId) : IRequest<Result>, IBaseRequest, ICommand;
