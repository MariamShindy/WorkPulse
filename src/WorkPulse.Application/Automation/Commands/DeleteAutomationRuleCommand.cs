using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Automation.Commands;

public sealed record DeleteAutomationRuleCommand(Guid RuleId) : IRequest<Result>, IBaseRequest, ICommand;
