namespace WorkPulse.Infrastructure.Services.Read;

internal sealed record TaskRowProjection(
	Guid TaskId,
	string Identifier,
	string Title,
	string TeamKey,
	string? ProjectKey,
	string Status,
	string Priority,
	Guid? AssigneeId,
	DateOnly? DueDate,
	DateTime CreatedAtUtc,
	Guid WorkflowStateId);
