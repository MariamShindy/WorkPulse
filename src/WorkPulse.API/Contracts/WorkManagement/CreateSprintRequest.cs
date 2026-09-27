using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Contracts.WorkManagement;

public sealed record CreateSprintRequest(Guid TeamId, string Name, string? Goal, DateOnly StartDate, DateOnly EndDate, SprintStatus Status);
