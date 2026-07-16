using WorkPulse.Domain.Enums;

namespace WorkPulse.API.Contracts.Automation;

public sealed record UpdateAutomationRuleRequest(string Name, AutomationTriggerType TriggerType, string TriggerConfigJson, AutomationActionType ActionType, string ActionConfigJson, bool IsEnabled);
