using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Auth.Commands.Register;
using WorkPulse.Application.Auth.Dtos;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Auth.Commands.Login;

public sealed class LoginCommandHandler(IUserIdentityService userIdentity, IJwtTokenService jwtTokenService) : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
	public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken ct)
	{
		Result<UserIdentityDto> loginResult = await userIdentity.ValidateCredentialsAsync(request.Email, request.Password, ct);
		if (loginResult.IsFailure)
		{
			return loginResult.Error;
		}
		UserIdentityDto user = loginResult.Value;
		if (!user.IsActive)
		{
			return Error.Unauthorized("Auth.UserInactive", "User account is inactive.");
		}
		string fullName = (user.FirstName + " " + user.LastName).Trim();
		var (accessToken, expiresAt) = jwtTokenService.GenerateAccessToken(user.Id, user.Email, fullName);
		var (refreshToken, _) = await jwtTokenService.GenerateRefreshTokenAsync(user.Id, ct);
		await userIdentity.UpdateLastLoginAsync(user.Id, ct);
		return new AuthResponseDto(accessToken, refreshToken, expiresAt, RegisterCommandHandler.ToProfile(user));
	}
}
