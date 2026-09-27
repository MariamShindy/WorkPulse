using WorkPulse.Domain.Common;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Domain.Entities;

public sealed class Notification : TenantEntity
{
	public Guid UserId { get; set; }

	public NotificationType Type { get; set; }

	public string Title { get; set; } = string.Empty;

	public string Body { get; set; } = string.Empty;

	public bool IsRead { get; set; }

	public string? RelatedEntityType { get; set; }

	public Guid? RelatedEntityId { get; set; }

	public Guid? ActorId { get; set; }
}
