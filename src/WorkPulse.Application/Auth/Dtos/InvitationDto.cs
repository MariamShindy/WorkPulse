using System;

namespace WorkPulse.Application.Auth.Dtos;

public sealed record InvitationDto(Guid Id, string Email, string Role, string Status, DateTime ExpiresAtUtc, DateTime? AcceptedAtUtc, DateTime CreatedAtUtc);
