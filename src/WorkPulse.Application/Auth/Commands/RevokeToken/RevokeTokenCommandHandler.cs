
namespace WorkPulse.Application.Auth.Commands.RevokeToken;

public sealed class RevokeTokenCommandHandler(IJwtTokenService jwtTokenService) : IRequestHandler<RevokeTokenCommand, Result>
{
	public async Task<Result> Handle(RevokeTokenCommand request, CancellationToken ct)
	{
		await jwtTokenService.RevokeRefreshTokenAsync(request.RefreshToken, ct);
		return Result.Success();
	}
}
