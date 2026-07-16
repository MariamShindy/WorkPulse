using System;

namespace WorkPulse.Application.Search.Dtos;

public sealed record SearchResultDto(Guid Id, string EntityType, string Title, string? Subtitle, string? Highlight, double Rank, Guid? TeamId, Guid? ProjectId);
