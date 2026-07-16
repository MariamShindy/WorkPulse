using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Application.Projects.Services;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Projects.Queries.GetTask;

public sealed class GetTaskQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<GetTaskQuery, Result<TaskItemDto>>
{
	public async Task<Result<TaskItemDto>> Handle(GetTaskQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		TaskItem task = await context.TaskItems.AsNoTracking().FirstOrDefaultAsync((TaskItem t) => t.Id == request.TaskId, ct);
		if (task == null)
		{
			return Error.NotFound("Task.NotFound", "Task not found.");
		}
		return await TaskDtoMapper.MapTaskDtoAsync(context, task, ct);
	}
}
