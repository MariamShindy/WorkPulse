using System;

namespace WorkPulse.API.Contracts.Projects;

public sealed record MoveTaskRequest(Guid WorkflowStateId, int? SortOrder);
