using System;

namespace WorkPulse.Application.Analytics.Dtos;

public sealed record ProjectProgressDto(Guid ProjectId, string ProjectKey, string ProjectName, int TotalTasks, int CompletedTasks, double CompletionRate);
