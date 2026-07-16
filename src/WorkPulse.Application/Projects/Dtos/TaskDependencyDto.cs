using System;

namespace WorkPulse.Application.Projects.Dtos;

public sealed record TaskDependencyDto(Guid Id, Guid TaskId, Guid DependsOnTaskId, string Type, DateTime CreatedAtUtc);
