using System;

namespace WorkPulse.Application.Abstractions;

public sealed record UserIdentityDto(Guid Id, string Email, string FirstName, string LastName, string? AvatarUrl, Guid? CurrentTenantId, DateTime CreatedAtUtc, DateTime? LastLoginAtUtc, bool IsActive);
