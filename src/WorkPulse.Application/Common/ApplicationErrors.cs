namespace WorkPulse.Application.Common;

public static class ApplicationErrors
{
	public static class Tenant
	{
		public const string RequiredCode = "Tenant.Required";

		public const string RequiredMessage = "X-Tenant-Id header is required.";
	}

	public static class Company
	{
		public const string NotFoundCode = "Company.NotFound";

		public const string SlugTakenCode = "Company.SlugTaken";

		public const string MemberNotFoundCode = "Company.MemberNotFound";

		public const string MemberExistsCode = "Company.MemberExists";
	}

	public static class Team
	{
		public const string NotFoundCode = "Team.NotFound";

		public const string KeyTakenCode = "Team.KeyTaken";

		public const string MemberNotFoundCode = "Team.MemberNotFound";

		public const string MemberExistsCode = "Team.MemberExists";
	}

	public static class Project
	{
		public const string NotFoundCode = "Project.NotFound";

		public const string KeyTakenCode = "Project.KeyTaken";
	}

	public static class Workflow
	{
		public const string NotFoundCode = "Workflow.NotFound";

		public const string StateNotFoundCode = "Workflow.StateNotFound";

		public const string StateInUseCode = "Workflow.StateInUse";

		public const string DefaultStateCode = "Workflow.DefaultStateRequired";
	}

	public static class Tasks
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

	public static class Label
	{
		public const string NotFoundCode = "Label.NotFound";

		public const string NameTakenCode = "Label.NameTaken";
	}

	public static class Epic
	{
		public const string NotFoundCode = "Epic.NotFound";
	}

	public static class Sprint
	{
		public const string NotFoundCode = "Sprint.NotFound";

		public const string InvalidDateRangeCode = "Sprint.InvalidDateRange";
	}

	public static class WorkLog
	{
		public const string NotFoundCode = "WorkLog.NotFound";
	}

	public static class SavedView
	{
		public const string NotFoundCode = "SavedView.NotFound";

		public const string ForbiddenCode = "SavedView.Forbidden";
	}

	public static class Auth
	{
		public const string UnauthorizedCode = "Auth.Unauthorized";

		public const string UnauthorizedMessage = "Authentication is required.";

		public const string InvalidCredentialsCode = "Auth.InvalidCredentials";

		public const string EmailTakenCode = "Auth.EmailTaken";

		public const string UserNotFoundCode = "Auth.UserNotFound";

		public const string UserInactiveCode = "Auth.UserInactive";

		public const string InvalidRefreshTokenCode = "Auth.InvalidRefreshToken";

		public const string RefreshTokenExpiredCode = "Auth.RefreshTokenExpired";

		public const string RegistrationFailedCode = "Auth.RegistrationFailed";
	}

	public static class Invitation
	{
		public const string NotFoundCode = "Invitation.NotFound";

		public const string ExpiredCode = "Invitation.Expired";

		public const string AlreadyAcceptedCode = "Invitation.AlreadyAccepted";

		public const string CancelledCode = "Invitation.Cancelled";

		public const string PendingExistsCode = "Invitation.PendingExists";

		public const string EmailMismatchCode = "Invitation.EmailMismatch";

		public const string RegistrationRequiredCode = "Invitation.RegistrationRequired";
	}

	public static class Collaboration
	{
		public const string CommentNotFoundCode = "Collaboration.CommentNotFound";

		public const string AlreadyWatchingCode = "Collaboration.AlreadyWatching";

		public const string NotWatchingCode = "Collaboration.NotWatching";

		public const string NotificationNotFoundCode = "Collaboration.NotificationNotFound";
	}

	public static class Files
	{
		public const string NotFoundCode = "Files.NotFound";

		public const string TooLargeCode = "Files.TooLarge";

		public const string InvalidTypeCode = "Files.InvalidType";
	}

	public static class Search
	{
		public const string QueryRequiredCode = "Search.QueryRequired";
	}
}
