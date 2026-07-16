using System;
using System.Collections.Generic;
using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Contracts.Projects;

public sealed record UpdateTaskRequest(string Title, string? Description, TaskPriority Priority, Guid? ProjectId, Guid? AssigneeId, IReadOnlyList<Guid>? AssigneeIds, DateOnly? DueDate, int? StoryPoints, decimal? EstimatedHours, bool IsBlocked, string? BlockedReason, Guid? EpicId, Guid? SprintId, Guid? AssignedTeamId, byte[]? RowVersion);
