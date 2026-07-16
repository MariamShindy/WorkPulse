using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Collaboration.Services;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Collaboration.Commands.UnwatchTask;

public sealed class UnwatchTaskCommandHandler(IApplicationDbContext context, ITenantContext tenantContext, ICurrentUserService currentUser, ITaskCollaborationService collaboration) : IRequestHandler<UnwatchTaskCommand, Result>
{
	public async Task<Result> Handle(UnwatchTaskCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
		{
			return Error.Unauthorized("Auth.Unauthorized", "Authentication is required.");
		}
		TaskWatcher watcher = await context.TaskWatchers.FirstOrDefaultAsync((TaskWatcher w) => w.TaskId == request.TaskId && w.UserId == currentUser.UserId.Value, ct);
		if (watcher == null)
		{
			return Error.NotFound("Collaboration.NotWatching", "Not watching this task.");
		}
		context.TaskWatchers.Remove(watcher);
		await collaboration.RecordActivityAsync(tenantContext.TenantId, request.TaskId, currentUser.UserId.Value, ActivityType.Unwatched, "Stopped watching", null, ct);
		return Result.Success();
	}
}
