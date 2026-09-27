using WorkPulse.Application.Collaboration.Services;

namespace WorkPulse.Application.Collaboration.Commands.WatchTask;

public sealed class WatchTaskCommandHandler(IApplicationDbContext context, ITenantContext tenantContext, ICurrentUserService currentUser, ITaskCollaborationService collaboration) : IRequestHandler<WatchTaskCommand, Result>
{
	public async Task<Result> Handle(WatchTaskCommand request, CancellationToken ct)
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
		if (!(await context.TaskItems.AnyAsync((TaskItem t) => t.Id == request.TaskId, ct)))
		{
			return Error.NotFound("Task.NotFound", "Task not found.");
		}
		if (await context.TaskWatchers.AnyAsync((TaskWatcher w) => w.TaskId == request.TaskId && w.UserId == currentUser.UserId.Value, ct))
		{
			return Error.Conflict("Collaboration.AlreadyWatching", "Already watching this task.");
		}
		context.TaskWatchers.Add(new TaskWatcher
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			TaskId = request.TaskId,
			UserId = currentUser.UserId.Value
		});
		await collaboration.RecordActivityAsync(tenantContext.TenantId, request.TaskId, currentUser.UserId.Value, ActivityType.Watched, "Started watching", null, ct);
		return Result.Success();
	}
}
