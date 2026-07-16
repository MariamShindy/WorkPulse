using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Projects.Commands.ReorderWorkflowStates;

public sealed class ReorderWorkflowStatesCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ReorderWorkflowStatesCommand, Result>
{
	public async Task<Result> Handle(ReorderWorkflowStatesCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		Workflow workflow = await context.Workflows.AsNoTracking().FirstOrDefaultAsync((Workflow w) => w.TeamId == request.TeamId && w.IsDefault, ct);
		if (workflow == null)
		{
			return Error.NotFound("Workflow.NotFound", "Workflow not found.");
		}
		List<WorkflowState> states = await context.WorkflowStates.Where((WorkflowState s) => s.WorkflowId == workflow.Id).ToListAsync(ct);
		int i;
		for (i = 0; i < request.StateIdsInOrder.Count; i++)
		{
			WorkflowState state = states.FirstOrDefault((WorkflowState s) => s.Id == request.StateIdsInOrder[i]);
			if (state != null)
			{
				state.Position = i;
			}
		}
		return Result.Success();
	}
}
