namespace WorkPulse.Infrastructure.Services.Read;

internal sealed record TaskMetricsRow(
	Guid TeamId,
	Guid WorkflowStateId,
	DateOnly? DueDate,
	DateTime CreatedAtUtc,
	DateTime? UpdatedAtUtc);
