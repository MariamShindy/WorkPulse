namespace WorkPulse.API.Contracts.Auth;

/// <summary>
/// Body for the refresh and logout endpoints. The token is normally read from the HttpOnly
/// <c>workpulse_refresh</c> cookie, so this is optional — it exists only so a client issued a
/// token before the cookie migration can still complete one final refresh. A non-nullable
/// property here would make MVC model validation reject the empty body the SPA now sends.
/// </summary>
public sealed record RefreshTokenRequest(string? RefreshToken = null);
