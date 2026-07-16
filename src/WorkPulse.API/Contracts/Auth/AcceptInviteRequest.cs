namespace WorkPulse.API.Contracts.Auth;

public sealed record AcceptInviteRequest(string Token, string? Password, string? FirstName, string? LastName);
