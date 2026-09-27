
namespace WorkPulse.Infrastructure.Persistence.Seed;

internal sealed record TaskSeed(
	string Team,
	string Title,
	string State,
	TaskPriority Priority,
	string? Project = null,
	string? Epic = null,
	int Sprint = -1,
	string? Assignee = null,
	string[]? CoAssignees = null,
	int? Points = null,
	decimal? EstHours = null,
	int? DueInDays = null,
	bool Blocked = false,
	string? BlockedReason = null,
	string[]? Labels = null,
	string? Parent = null,
	int AgeDays = 20);
