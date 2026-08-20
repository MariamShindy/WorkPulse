
namespace WorkPulse.Application.Collaboration.Dtos;

public sealed record NotificationDto(Guid Id, string Type, string Title, string Body, bool IsRead, string? RelatedEntityType, Guid? RelatedEntityId, Guid? ActorId, DateTime CreatedAtUtc);
