
namespace WorkPulse.Application.Auth.Dtos;

public sealed record AuthResponseDto(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAtUtc, UserProfileDto User);
