using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.WorkManagement.Commands.AssignLabelToTask;

public sealed class AssignLabelToTaskCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<AssignLabelToTaskCommand, Result>
{
	public async Task<Result> Handle(AssignLabelToTaskCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		if (!(await context.TaskItems.AnyAsync((TaskItem t) => t.Id == request.TaskId, ct)))
		{
			return Error.NotFound("Task.NotFound", "Task not found.");
		}
		if (!(await context.Labels.AnyAsync((Label l) => l.Id == request.LabelId, ct)))
		{
			return Error.NotFound("Label.NotFound", "Label not found.");
		}
		if (await context.TaskLabels.AnyAsync((TaskLabel tl) => tl.TaskId == request.TaskId && tl.LabelId == request.LabelId, ct))
		{
			return Error.Conflict("Task.LabelAlreadyAssigned", "Label is already assigned to this task.");
		}
		context.TaskLabels.Add(new TaskLabel
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			TaskId = request.TaskId,
			LabelId = request.LabelId
		});
		return Result.Success();
	}
}
