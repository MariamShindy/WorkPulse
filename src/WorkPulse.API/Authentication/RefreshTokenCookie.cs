namespace WorkPulse.API.Authentication;

/// <summary>
/// Carries the refresh token in an HttpOnly cookie instead of the response body.
/// <para>
/// The SPA previously kept a 30-day refresh token in <c>localStorage</c>, where any XSS could
/// read it and mint access tokens indefinitely. A cookie marked HttpOnly is unreadable from
/// JavaScript, so the same XSS can no longer exfiltrate a durable credential.
/// </para>
/// <para>
/// <c>SameSite=Strict</c> plus a path restricted to the auth endpoints is what stands in for
/// CSRF protection here: the cookie is only ever attached to same-site requests to
/// <c>/api/auth</c>, and every one of those endpoints rotates or revokes the token it receives.
/// </para>
/// </summary>
public static class RefreshTokenCookie
{
	public const string Name = "workpulse_refresh";

	/// <summary>Scoped so the cookie is not sent with ordinary API traffic.</summary>
	private const string CookiePath = "/api/auth";

	public static void Append(HttpContext context, string refreshToken, IConfiguration configuration)
	{
		int expiryDays = configuration.GetValue("JwtSettings:RefreshTokenExpiryDays", 30);

		context.Response.Cookies.Append(Name, refreshToken, BuildOptions(context, expiryDays));
	}

	public static void Delete(HttpContext context)
	{
		// Must match the original path/flags or the browser keeps the old cookie.
		CookieOptions options = BuildOptions(context, expiryDays: 0);
		options.Expires = DateTimeOffset.UnixEpoch;

		context.Response.Cookies.Append(Name, string.Empty, options);
	}

	/// <summary>
	/// The token from the cookie, falling back to an explicitly supplied body value so a client
	/// holding a token issued before this change can still complete one final refresh.
	/// </summary>
	public static string? Read(HttpContext context, string? fromBody)
	{
		string? fromCookie = context.Request.Cookies[Name];

		return !string.IsNullOrWhiteSpace(fromCookie)
			? fromCookie
			: (string.IsNullOrWhiteSpace(fromBody) ? null : fromBody);
	}

	private static CookieOptions BuildOptions(HttpContext context, int expiryDays)
	{
		return new CookieOptions
		{
			HttpOnly = true,
			// Browsers treat localhost as a secure context, so this holds in development too.
			Secure = context.Request.IsHttps || !IsLocalhost(context),
			SameSite = SameSiteMode.Strict,
			Path = CookiePath,
			Expires = expiryDays > 0 ? DateTimeOffset.UtcNow.AddDays(expiryDays) : null,
			IsEssential = true
		};
	}

	private static bool IsLocalhost(HttpContext context)
	{
		return string.Equals(context.Request.Host.Host, "localhost", StringComparison.OrdinalIgnoreCase);
	}
}
