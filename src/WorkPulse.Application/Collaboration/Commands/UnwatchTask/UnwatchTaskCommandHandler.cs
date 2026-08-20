using WorkPulse.Application.Collaboration.Services;

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
		TaskWatcher? watcher = await context.TaskWatchers.FirstOrDefaultAsync((TaskWatcher w) => w.TaskId == request.TaskId && w.UserId == currentUser.UserId.Value, ct);
		if (watcher is null)
		{
			return Error.NotFound("Collaboration.NotWatching", "Not watching this task.");
		}
		context.TaskWatchers.Remove(watcher);
		await collaboration.RecordActivityAsync(tenantContext.TenantId, request.TaskId, currentUser.UserId.Value, ActivityType.Unwatched, "Stopped watching", null, ct);
		return Result.Success();
	}
}
