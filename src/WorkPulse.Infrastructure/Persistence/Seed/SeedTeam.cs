
namespace WorkPulse.Infrastructure.Persistence.Seed;

internal sealed record SeedTeam(Team Team, Dictionary<string, WorkflowState> StatesByName, TeamIssueCounter Counter);
