using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Contracts.WorkManagement;

public sealed record UpdateSprintRequest(string Name, string? Goal, DateOnly StartDate, DateOnly EndDate, SprintStatus Status);
