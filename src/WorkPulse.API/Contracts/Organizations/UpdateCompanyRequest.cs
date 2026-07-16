namespace WorkPulse.API.Contracts.Organizations;

public sealed record UpdateCompanyRequest(string Name, string? Description, string? LogoUrl);
