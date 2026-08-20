using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Commands.CreateWorkflowState;

public sealed record CreateWorkflowStateCommand(Guid TeamId, string Name, WorkflowStateType Type, string Color, int Position) : IRequest<Result<WorkflowStateDto>>, IBaseRequest, ICommand;
