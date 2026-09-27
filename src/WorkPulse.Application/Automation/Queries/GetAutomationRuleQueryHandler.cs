using WorkPulse.Application.Automation.Commands;
using WorkPulse.Application.Automation.Dtos;

namespace WorkPulse.Application.Automation.Queries;

public sealed class GetAutomationRuleQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<GetAutomationRuleQuery, Result<AutomationRuleDto>>
{
	public async Task<Result<AutomationRuleDto>> Handle(GetAutomationRuleQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		AutomationRule? rule = await context.AutomationRules.AsNoTracking().FirstOrDefaultAsync((AutomationRule r) => r.Id == request.RuleId, ct);
		if (rule is null)
		{
			return Error.NotFound("Automation.NotFound", "Automation rule not found.");
		}
		return CreateAutomationRuleCommandHandler.MapToDto(rule);
	}
}
