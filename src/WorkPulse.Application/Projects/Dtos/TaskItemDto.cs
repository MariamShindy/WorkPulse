using System;
using System.Collections.Generic;

namespace WorkPulse.Application.Projects.Dtos;

public sealed record TaskItemDto(Guid Id, Guid TeamId, string TeamKey, string Identifier, Guid? ProjectId, string? ProjectKey, Guid WorkflowStateId, string WorkflowStateName, string WorkflowStateType, string Title, string? Description, string Priority, Guid? AssigneeId, IReadOnlyList<Guid> AssigneeIds, Guid CreatorId, DateOnly? DueDate, Guid? ParentTaskId, int SortOrder, int? StoryPoints, decimal? EstimatedHours, decimal LoggedHours, bool IsBlocked, string? BlockedReason, Guid? EpicId, Guid? SprintId, Guid? AssignedTeamId, DateTime CreatedAtUtc);
