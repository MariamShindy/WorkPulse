
namespace WorkPulse.Application.WorkManagement.Dtos;

public sealed record SprintDto(Guid Id, Guid TeamId, string Name, string? Goal, DateOnly StartDate, DateOnly EndDate, string Status, DateTime CreatedAtUtc);
