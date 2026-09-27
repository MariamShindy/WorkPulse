using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Application.Projects.Services;

namespace WorkPulse.Application.Projects.Queries.GetTask;

public sealed class GetTaskQueryHandler(IApplicationDbContext context, ITenantContext tenantContext)
	: IRequestHandler<GetTaskQuery, Result<TaskItemDto>>
{
	public async Task<Result<TaskItemDto>> Handle(GetTaskQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}

		TaskItem? task = await context.TaskItems.AsNoTracking()
			.ForTenant(tenantContext)
			.FirstOrDefaultAsync(item => item.Id == request.TaskId, ct);

		if (task is null)
		{
			return Error.NotFound(TaskErrors.NotFoundCode, "Task not found.");
		}

		return await TaskDtoMapper.MapTaskDtoAsync(context, task, ct);
	}
}
