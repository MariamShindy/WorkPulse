
namespace WorkPulse.Application.Analytics.Dtos;

public sealed record ThroughputPointDto(DateOnly PeriodStart, int CompletedTasks, int CompletedStoryPoints);
