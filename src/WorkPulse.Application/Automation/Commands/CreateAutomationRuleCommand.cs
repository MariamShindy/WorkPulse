using WorkPulse.Application.Automation.Dtos;
using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Automation.Commands;

public sealed record CreateAutomationRuleCommand(string Name, AutomationTriggerType TriggerType, string TriggerConfigJson, AutomationActionType ActionType, string ActionConfigJson, bool IsEnabled = true) : IRequest<Result<AutomationRuleDto>>, IBaseRequest, ICommand;
