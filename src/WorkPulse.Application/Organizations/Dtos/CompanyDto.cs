using System;

namespace WorkPulse.Application.Organizations.Dtos;

public sealed record CompanyDto(Guid Id, string Name, string Slug, string? LogoUrl, string? Description, bool IsActive, DateTime CreatedAtUtc);
