using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.WorkManagement.Commands.AssignLabelToTask;

public sealed record AssignLabelToTaskCommand(Guid TaskId, Guid LabelId) : IRequest<Result>, IBaseRequest, ICommand;
