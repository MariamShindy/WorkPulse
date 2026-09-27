
namespace WorkPulse.Application.WorkManagement.Commands.DeleteEpic;

public sealed class DeleteEpicCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<DeleteEpicCommand, Result>
{
	public async Task<Result> Handle(DeleteEpicCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		Epic? epic = await context.Epics.FirstOrDefaultAsync((Epic e) => e.Id == request.EpicId, ct);
		if (epic is null)
		{
			return Error.NotFound("Epic.NotFound", "Epic not found.");
		}
		foreach (TaskItem task in await context.TaskItems.Where((TaskItem t) => t.EpicId == request.EpicId).ToListAsync(ct))
		{
			task.EpicId = null;
		}
		context.Epics.Remove(epic);
		return Result.Success();
	}
}
