using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Auth.Commands.Register;
using WorkPulse.Application.Auth.Dtos;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(IUserIdentityService userIdentity, IJwtTokenService jwtTokenService) : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
	public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken ct)
	{
		Result<Guid> validation = await jwtTokenService.ValidateRefreshTokenAsync(request.RefreshToken, ct);
		if (validation.IsFailure)
		{
			return validation.Error;
		}
		Guid userId = validation.Value;
		UserIdentityDto user = await userIdentity.GetByIdAsync(userId, ct);
		if ((object)user == null)
		{
			return Error.NotFound("Auth.UserNotFound", "User not found.");
		}
		if (!user.IsActive)
		{
			return Error.Unauthorized("Auth.UserInactive", "User account is inactive.");
		}
		await jwtTokenService.RevokeRefreshTokenAsync(request.RefreshToken, ct);
		string fullName = (user.FirstName + " " + user.LastName).Trim();
		var (accessToken, expiresAt) = jwtTokenService.GenerateAccessToken(user.Id, user.Email, fullName);
		var (refreshToken, _) = await jwtTokenService.GenerateRefreshTokenAsync(user.Id, ct);
		return new AuthResponseDto(accessToken, refreshToken, expiresAt, RegisterCommandHandler.ToProfile(user));
	}
}
