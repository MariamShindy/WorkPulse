namespace WorkPulse.Application.Common;

public static class WorkflowStateTransition
{
	public static void Apply(TaskItem task, WorkflowStateType newStateType)
	{
		DateTime now = DateTime.UtcNow;

		if (newStateType == WorkflowStateType.Started && task.StartedAtUtc is null)
		{
			task.StartedAtUtc = now;
		}

		if (newStateType == WorkflowStateType.Completed)
		{
			task.CompletedAtUtc ??= now;
		}
		else
		{
			task.CompletedAtUtc = null;
		}
	}
}
