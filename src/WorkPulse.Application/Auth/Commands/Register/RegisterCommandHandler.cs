using WorkPulse.Application.Auth.Dtos;

namespace WorkPulse.Application.Auth.Commands.Register;

public sealed class RegisterCommandHandler(IUserIdentityService userIdentity, IJwtTokenService jwtTokenService) : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
	public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken ct)
	{
		Result<UserIdentityDto> registerResult = await userIdentity.RegisterAsync(request.Email, request.Password, request.FirstName, request.LastName, ct);
		if (registerResult.IsFailure)
		{
			return registerResult.Error;
		}
		UserIdentityDto user = registerResult.Value;
		return await BuildAuthResponseAsync(user, ct);
	}

	private async Task<AuthResponseDto> BuildAuthResponseAsync(UserIdentityDto user, CancellationToken ct)
	{
		string fullName = (user.FirstName + " " + user.LastName).Trim();
		var (accessToken, expiresAt) = jwtTokenService.GenerateAccessToken(user.Id, user.Email, fullName);
		var (refreshToken, _) = await jwtTokenService.GenerateRefreshTokenAsync(user.Id, ct);
		await userIdentity.UpdateLastLoginAsync(user.Id, ct);
		return new AuthResponseDto(accessToken, refreshToken, expiresAt, ToProfile(user));
	}

	internal static UserProfileDto ToProfile(UserIdentityDto user)
	{
		return new UserProfileDto(user.Id, user.Email, user.FirstName, user.LastName, user.AvatarUrl, user.CurrentTenantId, user.CreatedAtUtc, user.LastLoginAtUtc);
	}
}
