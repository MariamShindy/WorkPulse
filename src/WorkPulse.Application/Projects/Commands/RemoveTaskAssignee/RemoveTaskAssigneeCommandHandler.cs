using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Projects.Commands.RemoveTaskAssignee;

public sealed class RemoveTaskAssigneeCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<RemoveTaskAssigneeCommand, Result>
{
	public async Task<Result> Handle(RemoveTaskAssigneeCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		TaskAssignee assignee = await context.TaskAssignees.FirstOrDefaultAsync((TaskAssignee a) => a.TaskId == request.TaskId && a.UserId == request.UserId, ct);
		if (assignee == null)
		{
			return Error.NotFound("Task.AssigneeNotFound", "User is not assigned to this task.");
		}
		context.TaskAssignees.Remove(assignee);
		TaskItem task = await context.TaskItems.FirstOrDefaultAsync((TaskItem t) => t.Id == request.TaskId, ct);
		if (task?.AssigneeId == request.UserId)
		{
			task.AssigneeId = null;
		}
		return Result.Success();
	}
}
