
namespace WorkPulse.Application.Audit.Dtos;

public sealed record AuditLogEntryDto(Guid Id, string EntityType, Guid EntityId, string Action, Guid? UserId, string? ChangesJson, DateTime Timestamp);
