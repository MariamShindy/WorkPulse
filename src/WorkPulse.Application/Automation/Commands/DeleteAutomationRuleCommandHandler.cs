
namespace WorkPulse.Application.Automation.Commands;

public sealed class DeleteAutomationRuleCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<DeleteAutomationRuleCommand, Result>
{
	public async Task<Result> Handle(DeleteAutomationRuleCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		AutomationRule? rule = await context.AutomationRules.FirstOrDefaultAsync((AutomationRule r) => r.Id == request.RuleId, ct);
		if (rule is null)
		{
			return Error.NotFound("Automation.NotFound", "Automation rule not found.");
		}
		context.AutomationRules.Remove(rule);
		return Result.Success();
	}
}
