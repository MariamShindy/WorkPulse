
namespace WorkPulse.Application.Abstractions;

public interface IJwtTokenService
{
	(string AccessToken, DateTime ExpiresAtUtc) GenerateAccessToken(Guid userId, string email, string fullName);

	Task<(string Token, DateTime ExpiresAtUtc)> GenerateRefreshTokenAsync(Guid userId, CancellationToken ct = default(CancellationToken));

	Task<Result<Guid>> ValidateRefreshTokenAsync(string token, CancellationToken ct = default(CancellationToken));

	Task RevokeRefreshTokenAsync(string token, CancellationToken ct = default(CancellationToken));
}
