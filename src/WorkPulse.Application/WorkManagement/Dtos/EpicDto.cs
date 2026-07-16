using System;

namespace WorkPulse.Application.WorkManagement.Dtos;

public sealed record EpicDto(Guid Id, Guid TeamId, string Title, string? Description, string Status, DateTime CreatedAtUtc);
