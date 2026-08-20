using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.WorkManagement.Commands.DeleteEpic;

public sealed record DeleteEpicCommand(Guid EpicId) : IRequest<Result>, IBaseRequest, ICommand;
