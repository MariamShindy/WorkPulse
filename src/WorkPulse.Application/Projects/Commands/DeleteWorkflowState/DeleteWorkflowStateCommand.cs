using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Projects.Commands.DeleteWorkflowState;

public sealed record DeleteWorkflowStateCommand(Guid TeamId, Guid StateId) : IRequest<Result>, IBaseRequest, ICommand;
