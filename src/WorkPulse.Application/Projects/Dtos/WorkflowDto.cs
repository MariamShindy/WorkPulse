
namespace WorkPulse.Application.Projects.Dtos;

public sealed record WorkflowDto(Guid Id, Guid TeamId, string Name, bool IsDefault, IReadOnlyList<WorkflowStateDto> States);
