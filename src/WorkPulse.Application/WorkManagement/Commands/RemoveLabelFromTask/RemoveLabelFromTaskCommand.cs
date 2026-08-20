using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.WorkManagement.Commands.RemoveLabelFromTask;

public sealed record RemoveLabelFromTaskCommand(Guid TaskId, Guid LabelId) : IRequest<Result>, IBaseRequest, ICommand;
