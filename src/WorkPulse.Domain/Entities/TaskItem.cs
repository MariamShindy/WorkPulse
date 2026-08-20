using WorkPulse.Domain.Common;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Domain.Entities;

public sealed class TaskItem : TenantEntity
{
	public Guid TeamId { get; set; }

	public Guid? ProjectId { get; set; }

	public Guid WorkflowStateId { get; set; }

	public int Number { get; set; }

	public string Title { get; set; } = string.Empty;

	public string? Description { get; set; }

	public TaskPriority Priority { get; set; } = TaskPriority.None;

	public Guid? AssigneeId { get; set; }

	public Guid CreatorId { get; set; }

	public DateOnly? DueDate { get; set; }

	public Guid? ParentTaskId { get; set; }

	public int SortOrder { get; set; }

	public int? StoryPoints { get; set; }

	public decimal? EstimatedHours { get; set; }

	public decimal LoggedHours { get; set; }

	public bool IsBlocked { get; set; }

	public string? BlockedReason { get; set; }

	public Guid? EpicId { get; set; }

	public Guid? SprintId { get; set; }

	public Guid? AssignedTeamId { get; set; }

	public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
