namespace WorkPulse.Application.Common;

public static class InvitationErrors
{
	public const string NotFoundCode = "Invitation.NotFound";
	public const string ExpiredCode = "Invitation.Expired";
	public const string AlreadyAcceptedCode = "Invitation.AlreadyAccepted";
	public const string CancelledCode = "Invitation.Cancelled";
	public const string PendingExistsCode = "Invitation.PendingExists";
	public const string EmailMismatchCode = "Invitation.EmailMismatch";
	public const string RegistrationRequiredCode = "Invitation.RegistrationRequired";
}
