using System;
using System.Collections.Generic;
using WorkPulse.Domain.Common;

namespace WorkPulse.Domain.Entities;

public sealed class TaskComment : TenantEntity
{
	public Guid TaskId { get; set; }

	public Guid AuthorId { get; set; }

	public string Body { get; set; } = string.Empty;

	public List<Guid> MentionedUserIds { get; set; } = new List<Guid>();

	public bool IsEdited { get; set; }
}
