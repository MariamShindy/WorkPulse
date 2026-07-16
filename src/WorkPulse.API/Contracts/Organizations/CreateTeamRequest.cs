namespace WorkPulse.API.Contracts.Organizations;

public sealed record CreateTeamRequest(string Name, string? Key, string? Description, string? Icon, string? Color);
