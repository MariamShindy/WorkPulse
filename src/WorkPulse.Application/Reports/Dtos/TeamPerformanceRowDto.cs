using System;

namespace WorkPulse.Application.Reports.Dtos;

public sealed record TeamPerformanceRowDto(Guid TeamId, string TeamKey, string TeamName, int TotalTasks, int CompletedTasks, int OverdueTasks, double CompletionRate, double AverageCycleTimeDays);
