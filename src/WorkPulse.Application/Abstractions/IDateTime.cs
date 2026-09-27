namespace WorkPulse.Application.Abstractions;

public interface IDateTime
{
	DateTime UtcNow { get; }

	DateOnly TodayUtc { get; }
}
