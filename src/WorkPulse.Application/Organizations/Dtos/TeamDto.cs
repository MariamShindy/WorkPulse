
namespace WorkPulse.Application.Organizations.Dtos;

public sealed record TeamDto(Guid Id, string Name, string Key, string? Description, string? Icon, string? Color, bool IsArchived, DateTime CreatedAtUtc);
