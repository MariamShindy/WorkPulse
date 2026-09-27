namespace WorkPulse.Application.WorkManagement.Queries.GetSprintBacklog;

internal sealed record BacklogRow(TaskItem Task, Team Team, WorkflowState State, Project? Project);
