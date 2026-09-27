using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using WorkPulse.API.Authentication;
using WorkPulse.API.Configuration;
using WorkPulse.API.Extensions;
using WorkPulse.Application.Auth.Commands.Login;
using WorkPulse.Application.Auth.Commands.RefreshToken;
using WorkPulse.Application.Auth.Commands.Register;
using WorkPulse.Application.Auth.Commands.RevokeToken;
using WorkPulse.Application.Auth.Dtos;

using WorkPulse.API.Contracts.Auth;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting(RateLimitingExtensions.AuthPolicy)]
public sealed class AuthController(ISender sender, IConfiguration configuration) : ControllerBase
{
	[HttpPost("register")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(AuthResponse), 200)]
	public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
	{
		Result<AuthResponseDto> result = await sender.Send(
			(IRequest<Result<AuthResponseDto>>)new RegisterCommand(request.Email, request.Password, request.FirstName, request.LastName),
			ct);

		return IssueSession(result);
	}

	[HttpPost("login")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(AuthResponse), 200)]
	public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
	{
		Result<AuthResponseDto> result = await sender.Send(
			(IRequest<Result<AuthResponseDto>>)new LoginCommand(request.Email, request.Password),
			ct);

		return IssueSession(result);
	}

	/// <summary>
	/// Exchanges the refresh token for a new session. The token is read from the HttpOnly
	/// cookie; the request body is only consulted for clients issued a token before that change.
	/// </summary>
	[HttpPost("refresh")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(AuthResponse), 200)]
	public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshTokenRequest? request, CancellationToken ct)
	{
		string? refreshToken = RefreshTokenCookie.Read(HttpContext, request?.RefreshToken);

		if (string.IsNullOrWhiteSpace(refreshToken))
		{
			// Clear any stale cookie so a client is not stuck retrying with an unusable token.
			RefreshTokenCookie.Delete(HttpContext);
			return Unauthorized(new ProblemDetails
			{
				Title = "Unauthorized",
				Detail = "No refresh token was supplied.",
				Status = StatusCodes.Status401Unauthorized,
				Extensions = { ["code"] = "Auth.MissingRefreshToken" }
			});
		}

		Result<AuthResponseDto> result = await sender.Send(
			(IRequest<Result<AuthResponseDto>>)new RefreshTokenCommand(refreshToken),
			ct);

		if (result.IsFailure)
		{
			RefreshTokenCookie.Delete(HttpContext);
		}

		return IssueSession(result);
	}

	[HttpPost("logout")]
	[Authorize(Policy = "RequireAuthenticated")]
	[ProducesResponseType(204)]
	public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest? request, CancellationToken ct)
	{
		string? refreshToken = RefreshTokenCookie.Read(HttpContext, request?.RefreshToken);

		// Drop the cookie regardless of whether revocation finds a matching token, so the
		// browser cannot keep replaying it.
		RefreshTokenCookie.Delete(HttpContext);

		if (string.IsNullOrWhiteSpace(refreshToken))
		{
			return NoContent();
		}

		return (await sender.Send((IRequest<Result>)new RevokeTokenCommand(refreshToken), ct)).ToActionResult();
	}

	/// <summary>Moves the refresh token into the cookie and strips it from the response body.</summary>
	private ActionResult<AuthResponse> IssueSession(Result<AuthResponseDto> result)
	{
		if (result.IsFailure)
		{
			return result.ToErrorActionResult();
		}

		RefreshTokenCookie.Append(HttpContext, result.Value.RefreshToken, configuration);

		return Ok(AuthResponse.From(result.Value));
	}
}
