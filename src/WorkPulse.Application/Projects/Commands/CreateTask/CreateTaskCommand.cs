using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Commands.CreateTask;

public sealed record CreateTaskCommand(Guid TeamId, string Title, string? Description, TaskPriority Priority, Guid? ProjectId, Guid? WorkflowStateId, Guid? AssigneeId, IReadOnlyList<Guid>? AssigneeIds, DateOnly? DueDate, Guid? ParentTaskId, int? StoryPoints, decimal? EstimatedHours, bool IsBlocked, string? BlockedReason, Guid? EpicId, Guid? SprintId, Guid? AssignedTeamId) : IRequest<Result<TaskItemDto>>, IBaseRequest, ICommand;
