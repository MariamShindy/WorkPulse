using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Projects.Commands.AddTaskDependency;

public sealed class AddTaskDependencyCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<AddTaskDependencyCommand, Result<TaskDependencyDto>>
{
	public async Task<Result<TaskDependencyDto>> Handle(AddTaskDependencyCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		if (request.TaskId == request.DependsOnTaskId)
		{
			return Error.Validation("Task.SelfDependency", "A task cannot depend on itself.");
		}
		if (!(await context.TaskItems.AnyAsync((TaskItem t) => t.Id == request.TaskId, ct)))
		{
			return Error.NotFound("Task.NotFound", "Task not found.");
		}
		if (!(await context.TaskItems.AnyAsync((TaskItem t) => t.Id == request.DependsOnTaskId, ct)))
		{
			return Error.NotFound("Task.NotFound", "Dependent task not found.");
		}
		if (await context.TaskDependencies.AnyAsync((TaskDependency d) => d.TaskId == request.TaskId && d.DependsOnTaskId == request.DependsOnTaskId && (int)d.Type == (int)request.Type, ct))
		{
			return Error.Conflict("Task.DependencyExists", "This dependency already exists.");
		}
		TaskDependency dependency = new TaskDependency
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			TaskId = request.TaskId,
			DependsOnTaskId = request.DependsOnTaskId,
			Type = request.Type
		};
		context.TaskDependencies.Add(dependency);
		return new TaskDependencyDto(dependency.Id, dependency.TaskId, dependency.DependsOnTaskId, dependency.Type.ToString(), dependency.CreatedAtUtc);
	}
}
