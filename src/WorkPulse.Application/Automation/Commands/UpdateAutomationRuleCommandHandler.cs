using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Automation.Dtos;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Automation.Commands;

public sealed class UpdateAutomationRuleCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<UpdateAutomationRuleCommand, Result<AutomationRuleDto>>
{
	public async Task<Result<AutomationRuleDto>> Handle(UpdateAutomationRuleCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		AutomationRule rule = await context.AutomationRules.FirstOrDefaultAsync((AutomationRule r) => r.Id == request.RuleId, ct);
		if (rule == null)
		{
			return Error.NotFound("Automation.NotFound", "Automation rule not found.");
		}
		rule.Name = request.Name.Trim();
		rule.TriggerType = request.TriggerType;
		rule.TriggerConfigJson = request.TriggerConfigJson;
		rule.ActionType = request.ActionType;
		rule.ActionConfigJson = request.ActionConfigJson;
		rule.IsEnabled = request.IsEnabled;
		return CreateAutomationRuleCommandHandler.MapToDto(rule);
	}
}
