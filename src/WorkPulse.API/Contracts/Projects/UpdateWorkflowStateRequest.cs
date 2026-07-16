using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Contracts.Projects;

public sealed record UpdateWorkflowStateRequest(string Name, WorkflowStateType Type, string Color, int Position, bool IsDefault);
