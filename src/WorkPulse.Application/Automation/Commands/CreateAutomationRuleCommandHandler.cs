using WorkPulse.Application.Automation.Dtos;

namespace WorkPulse.Application.Automation.Commands;

public sealed class CreateAutomationRuleCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<CreateAutomationRuleCommand, Result<AutomationRuleDto>>
{
	public async Task<Result<AutomationRuleDto>> Handle(CreateAutomationRuleCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		AutomationRule rule = new AutomationRule
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			Name = request.Name.Trim(),
			TriggerType = request.TriggerType,
			TriggerConfigJson = request.TriggerConfigJson,
			ActionType = request.ActionType,
			ActionConfigJson = request.ActionConfigJson,
			IsEnabled = request.IsEnabled
		};
		context.AutomationRules.Add(rule);
		return MapToDto(rule);
	}

	internal static AutomationRuleDto MapToDto(AutomationRule rule)
	{
		return new AutomationRuleDto(rule.Id, rule.Name, rule.TriggerType.ToString(), rule.TriggerConfigJson, rule.ActionType.ToString(), rule.ActionConfigJson, rule.IsEnabled, rule.CreatedAtUtc);
	}
}
