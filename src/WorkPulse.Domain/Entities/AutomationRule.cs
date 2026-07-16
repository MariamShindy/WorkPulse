using WorkPulse.Domain.Common;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Domain.Entities;

public sealed class AutomationRule : TenantEntity
{
	public string Name { get; set; } = string.Empty;

	public AutomationTriggerType TriggerType { get; set; }

	public string TriggerConfigJson { get; set; } = "{}";

	public AutomationActionType ActionType { get; set; }

	public string ActionConfigJson { get; set; } = "{}";

	public bool IsEnabled { get; set; } = true;
}
