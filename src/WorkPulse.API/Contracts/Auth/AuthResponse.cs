using WorkPulse.Application.Auth.Dtos;

namespace WorkPulse.API.Contracts.Auth;

/// <summary>
/// The wire shape of an authentication response. Deliberately omits the refresh token — it is
/// delivered as an HttpOnly cookie so that JavaScript (and therefore any XSS) cannot read it.
/// </summary>
public sealed record AuthResponse(
	string AccessToken,
	DateTime AccessTokenExpiresAtUtc,
	UserProfileDto User)
{
	public static AuthResponse From(AuthResponseDto dto) =>
		new(dto.AccessToken, dto.AccessTokenExpiresAtUtc, dto.User);
}
