using System;
using System.Collections.Generic;
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

namespace WorkPulse.Application.Projects.Commands.CreateTask;

public sealed class CreateTaskCommandHandler(IApplicationDbContext context, ITenantContext tenantContext, ICurrentUserService currentUser) : IRequestHandler<CreateTaskCommand, Result<TaskItemDto>>
{
	public async Task<Result<TaskItemDto>> Handle(CreateTaskCommand request, CancellationToken ct)
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
		if (await context.Teams.AsNoTracking().FirstOrDefaultAsync((Team t) => t.Id == request.TeamId, ct) == null)
		{
			return Error.NotFound("Team.NotFound", "Team not found.");
		}
		bool hasValue = request.ProjectId.HasValue;
		bool flag = hasValue;
		if (flag)
		{
			flag = !(await context.Projects.AnyAsync((Project p) => p.Id == request.ProjectId && p.TeamId == request.TeamId, ct));
		}
		if (flag)
		{
			return Error.NotFound("Project.NotFound", "Project not found.");
		}
		bool hasValue2 = request.EpicId.HasValue;
		bool flag2 = hasValue2;
		if (flag2)
		{
			flag2 = !(await context.Epics.AnyAsync((Epic e) => e.Id == request.EpicId && e.TeamId == request.TeamId, ct));
		}
		if (flag2)
		{
			return Error.NotFound("Epic.NotFound", "Epic not found.");
		}
		bool hasValue3 = request.SprintId.HasValue;
		bool flag3 = hasValue3;
		if (flag3)
		{
			flag3 = !(await context.Sprints.AnyAsync((Sprint s) => s.Id == request.SprintId && s.TeamId == request.TeamId, ct));
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
		Guid workflowStateId;
		if (request.WorkflowStateId.HasValue)
		{
			WorkflowState workflowState = await context.WorkflowStates.AsNoTracking().FirstOrDefaultAsync((WorkflowState s) => s.Id == request.WorkflowStateId, ct);
			if (workflowState == null)
			{
				return Error.NotFound("Workflow.StateNotFound", "Workflow state not found.");
			}
			workflowStateId = workflowState.Id;
		}
		else
		{
			WorkflowState workflowState = await (from w in context.Workflows.AsNoTracking()
				join s in context.WorkflowStates.AsNoTracking() on w.Id equals s.WorkflowId
				where w.TeamId == request.TeamId && w.IsDefault && s.IsDefault
				select s).FirstOrDefaultAsync(ct);
			if (workflowState == null)
			{
				return Error.NotFound("Workflow.StateNotFound", "Default workflow state not found.");
			}
			workflowStateId = workflowState.Id;
		}
		TeamIssueCounter counter = await context.TeamIssueCounters.FirstOrDefaultAsync((TeamIssueCounter c) => c.TeamId == request.TeamId, ct);
		if (counter == null)
		{
			return Error.NotFound("Team.NotFound", "Team issue counter not found.");
		}
		counter.LastNumber++;
		int number = counter.LastNumber;
		List<Guid> assigneeIds = request.AssigneeIds?.ToList() ?? new List<Guid>();
		Guid primaryAssignee = request.AssigneeId ?? assigneeIds.FirstOrDefault();
		TaskItem task = new TaskItem
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			TeamId = request.TeamId,
			ProjectId = request.ProjectId,
			WorkflowStateId = workflowStateId,
			Number = number,
			Title = request.Title.Trim(),
			Description = request.Description?.Trim(),
			Priority = request.Priority,
			AssigneeId = ((primaryAssignee == Guid.Empty) ? ((Guid?)null) : new Guid?(primaryAssignee)),
			CreatorId = currentUser.UserId.Value,
			DueDate = request.DueDate,
			ParentTaskId = request.ParentTaskId,
			StoryPoints = request.StoryPoints,
			EstimatedHours = request.EstimatedHours,
			IsBlocked = request.IsBlocked,
			BlockedReason = request.BlockedReason?.Trim(),
			EpicId = request.EpicId,
			SprintId = request.SprintId,
			AssignedTeamId = request.AssignedTeamId
		};
		context.TaskItems.Add(task);
		if (assigneeIds.Count > 0)
		{
			await TaskDtoMapper.SyncAssigneesAsync(context, tenantContext.TenantId, task.Id, assigneeIds, ct);
		}
		else if (primaryAssignee != Guid.Empty)
		{
			await TaskDtoMapper.SyncAssigneesAsync(context, tenantContext.TenantId, task.Id, [primaryAssignee], ct);
		}
		return await TaskDtoMapper.MapTaskDtoAsync(context, task, ct);
	}
}
