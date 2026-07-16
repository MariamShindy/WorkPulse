namespace WorkPulse.API.Contracts.Organizations;

public sealed record UpdateTeamRequest(string Name, string? Description, string? Icon, string? Color);
