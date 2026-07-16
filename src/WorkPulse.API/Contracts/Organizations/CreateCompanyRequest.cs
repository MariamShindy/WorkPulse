namespace WorkPulse.API.Contracts.Organizations;

public sealed record CreateCompanyRequest(string Name, string? Description, string? LogoUrl);
