
namespace WorkPulse.Application.WorkManagement.Commands.DeleteWorkLog;

public sealed class DeleteWorkLogCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<DeleteWorkLogCommand, Result>
{
	public async Task<Result> Handle(DeleteWorkLogCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		WorkLog? workLog = await context.WorkLogs.FirstOrDefaultAsync((WorkLog w) => w.Id == request.WorkLogId, ct);
		if (workLog is null)
		{
			return Error.NotFound("WorkLog.NotFound", "Work log not found.");
		}
		TaskItem? task = await context.TaskItems.FirstOrDefaultAsync((TaskItem t) => t.Id == workLog.TaskId, ct);
		if (task != null)
		{
			task.LoggedHours = Math.Max(0m, task.LoggedHours - workLog.Hours);
		}
		context.WorkLogs.Remove(workLog);
		return Result.Success();
	}
}
