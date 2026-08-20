namespace WorkPulse.Application.Common;

public static class TaskErrors
{
	public const string NotFoundCode = "Task.NotFound";
	public const string InvalidStateCode = "Task.InvalidState";
	public const string ConcurrencyCode = "Task.ConcurrencyConflict";
	public const string SelfDependencyCode = "Task.SelfDependency";
	public const string DependencyExistsCode = "Task.DependencyExists";
	public const string DependencyNotFoundCode = "Task.DependencyNotFound";
	public const string AssigneeExistsCode = "Task.AssigneeExists";
	public const string AssigneeNotFoundCode = "Task.AssigneeNotFound";
	public const string LabelAlreadyAssignedCode = "Task.LabelAlreadyAssigned";
	public const string LabelNotAssignedCode = "Task.LabelNotAssigned";
}
