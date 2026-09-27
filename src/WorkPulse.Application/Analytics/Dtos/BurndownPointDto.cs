
namespace WorkPulse.Application.Analytics.Dtos;

public sealed record BurndownPointDto(DateOnly Date, double Remaining, double IdealRemaining, double CompletedCumulative);
