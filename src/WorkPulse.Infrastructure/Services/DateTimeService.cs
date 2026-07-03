using WorkPulse.Application.Abstractions;

namespace WorkPulse.Infrastructure.Services;

public sealed class DateTimeService : IDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly TodayUtc => DateOnly.FromDateTime(DateTime.UtcNow);
}
