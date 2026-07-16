using System;

namespace WorkPulse.Application.Projects.Dtos;

public sealed record TaskAssigneeDto(Guid Id, Guid TaskId, Guid UserId, DateTime CreatedAtUtc);
