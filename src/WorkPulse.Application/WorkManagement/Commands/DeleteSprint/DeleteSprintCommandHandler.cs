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

namespace WorkPulse.Application.WorkManagement.Commands.DeleteSprint;

public sealed class DeleteSprintCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<DeleteSprintCommand, Result>
{
	public async Task<Result> Handle(DeleteSprintCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		Sprint sprint = await context.Sprints.FirstOrDefaultAsync((Sprint s) => s.Id == request.SprintId, ct);
		if (sprint == null)
		{
			return Error.NotFound("Sprint.NotFound", "Sprint not found.");
		}
		foreach (TaskItem task in await context.TaskItems.Where((TaskItem t) => t.SprintId == request.SprintId).ToListAsync(ct))
		{
			task.SprintId = null;
		}
		context.Sprints.Remove(sprint);
		return Result.Success();
	}
}
