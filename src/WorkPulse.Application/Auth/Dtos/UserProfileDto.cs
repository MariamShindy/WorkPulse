using System;

namespace WorkPulse.Application.Auth.Dtos;

public sealed record UserProfileDto(Guid Id, string Email, string FirstName, string LastName, string? AvatarUrl, Guid? CurrentTenantId, DateTime CreatedAtUtc, DateTime? LastLoginAtUtc);
