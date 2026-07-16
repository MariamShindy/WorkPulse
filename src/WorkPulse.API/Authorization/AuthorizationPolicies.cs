namespace WorkPulse.API.Authorization;

public static class AuthorizationPolicies
{
	public const string RequireAuthenticated = "RequireAuthenticated";

	public const string RequireCompanyAdmin = "RequireCompanyAdmin";

	public const string RequireCompanyOwner = "RequireCompanyOwner";
}
