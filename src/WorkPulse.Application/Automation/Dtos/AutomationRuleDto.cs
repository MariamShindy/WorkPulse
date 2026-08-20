
namespace WorkPulse.Application.Automation.Dtos;

public sealed record AutomationRuleDto(Guid Id, string Name, string TriggerType, string TriggerConfigJson, string ActionType, string ActionConfigJson, bool IsEnabled, DateTime CreatedAtUtc);
