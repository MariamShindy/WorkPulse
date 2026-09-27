
namespace WorkPulse.Application.WorkManagement.Commands.RemoveLabelFromTask;

public sealed class RemoveLabelFromTaskCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<RemoveLabelFromTaskCommand, Result>
{
	public async Task<Result> Handle(RemoveLabelFromTaskCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		TaskLabel? taskLabel = await context.TaskLabels.FirstOrDefaultAsync((TaskLabel tl) => tl.TaskId == request.TaskId && tl.LabelId == request.LabelId, ct);
		if (taskLabel is null)
		{
			return Error.NotFound("Task.LabelNotAssigned", "Label is not assigned to this task.");
		}
		context.TaskLabels.Remove(taskLabel);
		return Result.Success();
	}
}
