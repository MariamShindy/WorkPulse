
namespace WorkPulse.API.Contracts.Projects;

public sealed record ReorderWorkflowStatesRequest(IReadOnlyList<Guid> StateIdsInOrder);
