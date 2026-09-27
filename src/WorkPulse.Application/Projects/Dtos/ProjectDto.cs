
namespace WorkPulse.Application.Projects.Dtos;

public sealed record ProjectDto(Guid Id, Guid TeamId, string TeamKey, string Name, string Key, string? Description, string Status, Guid? LeadId, DateOnly? StartDate, DateOnly? TargetDate, bool IsArchived, DateTime CreatedAtUtc);
