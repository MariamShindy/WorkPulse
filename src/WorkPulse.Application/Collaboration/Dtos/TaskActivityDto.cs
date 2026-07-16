using System;

namespace WorkPulse.Application.Collaboration.Dtos;

public sealed record TaskActivityDto(Guid Id, Guid TaskId, Guid ActorId, string Type, string? Summary, string? MetadataJson, DateTime CreatedAtUtc);
