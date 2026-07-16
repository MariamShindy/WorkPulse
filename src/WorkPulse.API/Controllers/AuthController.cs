using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkPulse.API.Extensions;
using WorkPulse.Application.Auth.Commands.Login;
using WorkPulse.Application.Auth.Commands.RefreshToken;
using WorkPulse.Application.Auth.Commands.Register;
using WorkPulse.Application.Auth.Commands.RevokeToken;
using WorkPulse.Application.Auth.Dtos;
using WorkPulse.Application.Common.Result;

using WorkPulse.API.Contracts.Auth;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
	[HttpPost("register")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(AuthResponseDto), 200)]
	public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<AuthResponseDto>>)new RegisterCommand(request.Email, request.Password, request.FirstName, request.LastName), ct)).ToActionResult();
	}

	[HttpPost("login")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(AuthResponseDto), 200)]
	public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<AuthResponseDto>>)new LoginCommand(request.Email, request.Password), ct)).ToActionResult();
	}

	[HttpPost("refresh")]
	[AllowAnonymous]
	[ProducesResponseType(typeof(AuthResponseDto), 200)]
	public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] RefreshTokenRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<AuthResponseDto>>)new RefreshTokenCommand(request.RefreshToken), ct)).ToActionResult();
	}

	[HttpPost("logout")]
	[Authorize(Policy = "RequireAuthenticated")]
	[ProducesResponseType(204)]
	public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new RevokeTokenCommand(request.RefreshToken), ct)).ToActionResult();
	}
}
