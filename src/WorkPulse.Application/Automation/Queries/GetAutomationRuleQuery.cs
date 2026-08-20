using WorkPulse.Application.Automation.Dtos;
using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Automation.Queries;

public sealed record GetAutomationRuleQuery(Guid RuleId) : IRequest<Result<AutomationRuleDto>>, IBaseRequest, IQuery;
