
namespace WorkPulse.Application.Projects.Commands.AddTaskAssignee;

public sealed class AddTaskAssigneeCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<AddTaskAssigneeCommand, Result>
{
	public async Task<Result> Handle(AddTaskAssigneeCommand request, CancellationToken ct)
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
		if (await context.TaskAssignees.AnyAsync((TaskAssignee a) => a.TaskId == request.TaskId && a.UserId == request.UserId, ct))
		{
			return Error.Conflict("Task.AssigneeExists", "User is already assigned to this task.");
		}
		context.TaskAssignees.Add(new TaskAssignee
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			TaskId = request.TaskId,
			UserId = request.UserId
		});
		return Result.Success();
	}
}
