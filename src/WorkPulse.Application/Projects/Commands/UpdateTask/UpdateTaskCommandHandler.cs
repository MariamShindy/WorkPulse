using System;
using System.Linq;
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

namespace WorkPulse.Application.Projects.Commands.UpdateTask;

public sealed class UpdateTaskCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<UpdateTaskCommand, Result<TaskItemDto>>
{
	public async Task<Result<TaskItemDto>> Handle(UpdateTaskCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		TaskItem task = await context.TaskItems.FirstOrDefaultAsync((TaskItem t) => t.Id == request.TaskId, ct);
		if (task == null)
		{
			return Error.NotFound("Task.NotFound", "Task not found.");
		}
		if (request.RowVersion != null && !task.RowVersion.SequenceEqual(request.RowVersion))
		{
			return Error.Conflict("Task.ConcurrencyConflict", "The task was modified by another user. Please refresh and try again.");
		}
		bool hasValue = request.ProjectId.HasValue;
		bool flag = hasValue;
		if (flag)
		{
			flag = !(await context.Projects.AnyAsync((Project p) => p.Id == request.ProjectId && p.TeamId == task.TeamId, ct));
		}
		if (flag)
		{
			return Error.NotFound("Project.NotFound", "Project not found.");
		}
		bool hasValue2 = request.EpicId.HasValue;
		bool flag2 = hasValue2;
		if (flag2)
		{
			flag2 = !(await context.Epics.AnyAsync((Epic e) => e.Id == request.EpicId && e.TeamId == task.TeamId, ct));
		}
		if (flag2)
		{
			return Error.NotFound("Epic.NotFound", "Epic not found.");
		}
		bool hasValue3 = request.SprintId.HasValue;
		bool flag3 = hasValue3;
		if (flag3)
		{
			flag3 = !(await context.Sprints.AnyAsync((Sprint s) => s.Id == request.SprintId && s.TeamId == task.TeamId, ct));
		}
		if (flag3)
		{
			return Error.NotFound("Sprint.NotFound", "Sprint not found.");
		}
		bool hasValue4 = request.AssignedTeamId.HasValue;
		bool flag4 = hasValue4;
		if (flag4)
		{
			flag4 = !(await context.Teams.AnyAsync((Team t) => t.Id == request.AssignedTeamId, ct));
		}
		if (flag4)
		{
			return Error.NotFound("Team.NotFound", "Assigned team not found.");
		}
		task.Title = request.Title.Trim();
		task.Description = request.Description?.Trim();
		task.Priority = request.Priority;
		task.ProjectId = request.ProjectId;
		task.AssigneeId = request.AssigneeId;
		task.DueDate = request.DueDate;
		task.StoryPoints = request.StoryPoints;
		task.EstimatedHours = request.EstimatedHours;
		task.IsBlocked = request.IsBlocked;
		task.BlockedReason = request.BlockedReason?.Trim();
		task.EpicId = request.EpicId;
		task.SprintId = request.SprintId;
		task.AssignedTeamId = request.AssignedTeamId;
		if (request.AssigneeIds != null)
		{
			await TaskDtoMapper.SyncAssigneesAsync(context, tenantContext.TenantId, task.Id, request.AssigneeIds, ct);
			task.AssigneeId = request.AssigneeIds.FirstOrDefault();
		}
		return await TaskDtoMapper.MapTaskDtoAsync(context, task, ct);
	}
}
