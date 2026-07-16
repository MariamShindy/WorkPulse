using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Contracts.Automation;

public sealed record CreateAutomationRuleRequest(string Name, AutomationTriggerType TriggerType, string TriggerConfigJson, AutomationActionType ActionType, string ActionConfigJson, bool IsEnabled = true);
