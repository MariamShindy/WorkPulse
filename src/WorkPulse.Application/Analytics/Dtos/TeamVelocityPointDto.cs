using System;

namespace WorkPulse.Application.Analytics.Dtos;

public sealed record TeamVelocityPointDto(DateOnly WeekStart, int Created, int Completed);
