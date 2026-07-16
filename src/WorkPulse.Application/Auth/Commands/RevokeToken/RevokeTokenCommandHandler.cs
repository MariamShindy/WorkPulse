using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Auth.Commands.RevokeToken;

public sealed class RevokeTokenCommandHandler(IJwtTokenService jwtTokenService) : IRequestHandler<RevokeTokenCommand, Result>
{
	public async Task<Result> Handle(RevokeTokenCommand request, CancellationToken ct)
	{
		await jwtTokenService.RevokeRefreshTokenAsync(request.RefreshToken, ct);
		return Result.Success();
	}
}
