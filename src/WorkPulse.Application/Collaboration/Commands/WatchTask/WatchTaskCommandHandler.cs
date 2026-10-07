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

		Guid userId = currentUser.UserId.Value;
		Guid tenantId = tenantContext.TenantId;

		// Unwatch soft-deletes; the unique (TaskId, UserId) index still holds the row.
		TaskWatcher? existing = await context.TaskWatchers
			.IgnoreQueryFilters()
			.FirstOrDefaultAsync(
				(TaskWatcher w) => w.TenantId == tenantId && w.TaskId == request.TaskId && w.UserId == userId,
				ct);

		if (existing is not null)
		{
			if (!existing.IsDeleted)
			{
				return Error.Conflict("Collaboration.AlreadyWatching", "Already watching this task.");
			}

			existing.IsDeleted = false;
			existing.DeletedAtUtc = null;
			existing.DeletedById = null;
			await collaboration.RecordActivityAsync(tenantId, request.TaskId, userId, ActivityType.Watched, "Started watching", null, ct);
			return Result.Success();
		}

		context.TaskWatchers.Add(new TaskWatcher
		{
			Id = Guid.NewGuid(),
			TenantId = tenantId,
			TaskId = request.TaskId,
			UserId = userId
		});
		await collaboration.RecordActivityAsync(tenantId, request.TaskId, userId, ActivityType.Watched, "Started watching", null, ct);
		return Result.Success();
	}
}
