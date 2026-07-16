using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;

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
		AutomationRule rule = await context.AutomationRules.FirstOrDefaultAsync((AutomationRule r) => r.Id == request.RuleId, ct);
		if (rule == null)
		{
			return Error.NotFound("Automation.NotFound", "Automation rule not found.");
		}
		context.AutomationRules.Remove(rule);
		return Result.Success();
	}
}
