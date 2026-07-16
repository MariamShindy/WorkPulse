using System;

namespace WorkPulse.Application.WorkManagement.Dtos;

public sealed record WorkLogDto(Guid Id, Guid TaskId, Guid UserId, decimal Hours, string? Description, DateOnly LoggedDate, DateTime CreatedAtUtc);
