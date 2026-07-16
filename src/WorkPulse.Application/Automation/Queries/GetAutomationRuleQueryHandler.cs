using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Automation.Commands;
using WorkPulse.Application.Automation.Dtos;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;

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
		AutomationRule rule = await context.AutomationRules.AsNoTracking().FirstOrDefaultAsync((AutomationRule r) => r.Id == request.RuleId, ct);
		if (rule == null)
		{
			return Error.NotFound("Automation.NotFound", "Automation rule not found.");
		}
		return CreateAutomationRuleCommandHandler.MapToDto(rule);
	}
}
