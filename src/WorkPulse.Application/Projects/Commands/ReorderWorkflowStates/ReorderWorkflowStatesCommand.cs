using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Projects.Commands.ReorderWorkflowStates;

public sealed record ReorderWorkflowStatesCommand(Guid TeamId, IReadOnlyList<Guid> StateIdsInOrder) : IRequest<Result>, IBaseRequest, ICommand;
