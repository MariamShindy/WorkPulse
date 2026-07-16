namespace WorkPulse.API.Contracts.Auth;

public sealed record RegisterRequest(string Email, string Password, string FirstName, string LastName);
