using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WorkPulse.Application.Abstractions.Persistence;

namespace WorkPulse.Infrastructure.Services;

public sealed class JwtTokenService(IApplicationDbContext context, IDateTime dateTime, IOptions<JwtSettings> jwtOptions) : IJwtTokenService
{
	private readonly JwtSettings _settings = jwtOptions.Value;

	public (string AccessToken, DateTime ExpiresAtUtc) GenerateAccessToken(Guid userId, string email, string fullName)
	{
		DateTime dateTime2 = dateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes);
		SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
		SigningCredentials signingCredentials = new SigningCredentials(key, "HS256");
		List<Claim> claims = new List<Claim>
		{
			new Claim("sub", userId.ToString()),
			new Claim("email", email),
			new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", userId.ToString()),
			new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", email),
			new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", fullName),
			new Claim("jti", Guid.NewGuid().ToString())
		};
		string issuer = _settings.Issuer;
		string audience = _settings.Audience;
		DateTime? expires = dateTime2;
		SigningCredentials signingCredentials2 = signingCredentials;
		JwtSecurityToken token = new JwtSecurityToken(issuer, audience, claims, null, expires, signingCredentials2);
		return (AccessToken: new JwtSecurityTokenHandler().WriteToken(token), ExpiresAtUtc: dateTime2);
	}

	public async Task<(string Token, DateTime ExpiresAtUtc)> GenerateRefreshTokenAsync(Guid userId, CancellationToken ct = default(CancellationToken))
	{
		string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
		DateTime expiresAt = dateTime.UtcNow.AddDays(_settings.RefreshTokenExpiryDays);
		DateTime now = dateTime.UtcNow;
		context.RefreshTokens.Add(new RefreshToken
		{
			Id = Guid.NewGuid(),
			UserId = userId,
			Token = token,
			ExpiresAtUtc = expiresAt,
			CreatedAtUtc = now
		});
		return (Token: token, ExpiresAtUtc: expiresAt);
	}

	public async Task<Result<Guid>> ValidateRefreshTokenAsync(string token, CancellationToken ct = default(CancellationToken))
	{
		RefreshToken? refreshToken = await context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync((RefreshToken t) => t.Token == token, ct);
		if (refreshToken == null)
		{
			return Error.Unauthorized("Auth.InvalidRefreshToken", "Invalid refresh token.");
		}
		if (refreshToken.RevokedAtUtc.HasValue)
		{
			return Error.Unauthorized("Auth.InvalidRefreshToken", "Refresh token has been revoked.");
		}
		if (refreshToken.ExpiresAtUtc <= dateTime.UtcNow)
		{
			return Error.Unauthorized("Auth.RefreshTokenExpired", "Refresh token has expired.");
		}
		return refreshToken.UserId;
	}

	public async Task RevokeRefreshTokenAsync(string token, CancellationToken ct = default(CancellationToken))
	{
		RefreshToken? refreshToken = await context.RefreshTokens.FirstOrDefaultAsync((RefreshToken t) => t.Token == token, ct);
		if (refreshToken != null && !refreshToken.RevokedAtUtc.HasValue)
		{
			refreshToken.RevokedAtUtc = dateTime.UtcNow;
		}
	}
}
