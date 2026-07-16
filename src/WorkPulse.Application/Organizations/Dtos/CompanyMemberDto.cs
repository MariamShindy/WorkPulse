using System;

namespace WorkPulse.Application.Organizations.Dtos;

public sealed record CompanyMemberDto(Guid Id, Guid UserId, string Email, string FullName, string Role, bool IsActive, DateTime CreatedAtUtc);
