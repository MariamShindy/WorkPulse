using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Projects.Commands.DeleteTask;

public sealed class DeleteTaskCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<DeleteTaskCommand, Result>
{
	public async Task<Result> Handle(DeleteTaskCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		TaskItem task = await context.TaskItems.FirstOrDefaultAsync((TaskItem t) => t.Id == request.TaskId, ct);
		if (task == null)
		{
			return Error.NotFound("Task.NotFound", "Task not found.");
		}
		context.TaskItems.Remove(task);
		return Result.Success();
	}
}
