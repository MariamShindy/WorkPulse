using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Contracts.Projects;

public sealed record CreateTaskRequest(Guid TeamId, string Title, string? Description, TaskPriority Priority, Guid? ProjectId, Guid? WorkflowStateId, Guid? AssigneeId, IReadOnlyList<Guid>? AssigneeIds, DateOnly? DueDate, Guid? ParentTaskId, int? StoryPoints, decimal? EstimatedHours, bool IsBlocked, string? BlockedReason, Guid? EpicId, Guid? SprintId, Guid? AssignedTeamId);
