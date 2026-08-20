using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.WorkManagement.Commands.DeleteLabel;

public sealed record DeleteLabelCommand(Guid LabelId) : IRequest<Result>, IBaseRequest, ICommand;
