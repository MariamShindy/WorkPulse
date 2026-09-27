namespace WorkPulse.Application.Common;

public static class AuthErrors
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
