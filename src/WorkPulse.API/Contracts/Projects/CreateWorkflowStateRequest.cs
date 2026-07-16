using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Contracts.Projects;

public sealed record CreateWorkflowStateRequest(string Name, WorkflowStateType Type, string Color, int Position);
