
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
		TaskItem? task = await context.TaskItems.FirstOrDefaultAsync((TaskItem t) => t.Id == request.TaskId, ct);
		if (task is null)
		{
			return Error.NotFound("Task.NotFound", "Task not found.");
		}
		context.TaskItems.Remove(task);
		return Result.Success();
	}
}
