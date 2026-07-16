namespace WorkPulse.API.Contracts.Auth;

public sealed record UpdateProfileRequest(string FirstName, string LastName, string? AvatarUrl);
