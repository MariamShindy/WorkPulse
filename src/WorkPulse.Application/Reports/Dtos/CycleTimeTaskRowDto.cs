
namespace WorkPulse.Application.Reports.Dtos;

public sealed record CycleTimeTaskRowDto(Guid TaskId, string Identifier, string Title, string TeamKey, Guid? AssigneeId, DateTime? StartedAtUtc, DateTime? CompletedAtUtc, double? CycleTimeDays, double? LeadTimeDays);
