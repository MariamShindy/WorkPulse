using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.WorkManagement.Commands.DeleteWorkLog;

public sealed record DeleteWorkLogCommand(Guid WorkLogId) : IRequest<Result>, IBaseRequest, ICommand;
