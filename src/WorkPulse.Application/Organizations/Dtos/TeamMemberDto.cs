using System;

namespace WorkPulse.Application.Organizations.Dtos;

public sealed record TeamMemberDto(Guid Id, Guid TeamId, Guid UserId, string Email, string FullName, string Role, DateTime CreatedAtUtc);
