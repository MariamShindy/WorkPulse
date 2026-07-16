using System;

namespace WorkPulse.Application.Organizations.Dtos;

public sealed record UserCompanyDto(Guid Id, string Name, string Slug, string? LogoUrl, string Role);
