using System;
using WorkPulse.Domain.Common;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Domain.Entities;

public sealed class SavedView : TenantEntity
{
	public Guid UserId { get; set; }

	public string Name { get; set; } = string.Empty;

	public SavedViewEntityType EntityType { get; set; }

	public string FiltersJson { get; set; } = "{}";

	public string SortJson { get; set; } = "{}";

	public bool IsShared { get; set; }
}
