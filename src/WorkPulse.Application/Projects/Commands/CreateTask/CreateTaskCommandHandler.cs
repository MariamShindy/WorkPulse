using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Application.Projects.Services;

namespace WorkPulse.Application.Projects.Commands.CreateTask;

public sealed class CreateTaskCommandHandler(
	IApplicationDbContext context,
	ITenantContext tenantContext,
	ICurrentUserService currentUser) : IRequestHandler<CreateTaskCommand, Result<TaskItemDto>>
{
	public async Task<Result<TaskItemDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}

		if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
		{
			return Error.Unauthorized(AuthErrors.UnauthorizedCode, AuthErrors.UnauthorizedMessage);
		}

		Result validation = await ValidateRelatedEntitiesAsync(request, cancellationToken);
		if (validation.IsFailure)
		{
			return validation.Error;
		}

		Result<(Guid StateId, WorkflowStateType StateType)> workflowStateResult = await ResolveWorkflowStateIdAsync(request, cancellationToken);
		if (workflowStateResult.IsFailure)
		{
			return workflowStateResult.Error;
		}

		TeamIssueCounter? issueCounter = await context.TeamIssueCounters
			.FirstOrDefaultAsync(counter => counter.TeamId == request.TeamId, cancellationToken);
		if (issueCounter is null)
		{
			return Error.NotFound(TeamErrors.NotFoundCode, "Team issue counter not found.");
		}

		TaskItem task = CreateTaskEntity(request, workflowStateResult.Value.StateId, issueCounter, currentUser.UserId.Value);
		WorkflowStateTransition.Apply(task, workflowStateResult.Value.StateType);
		context.TaskItems.Add(task);

		await SyncAssigneesAsync(request, task.Id, cancellationToken);
		return await TaskDtoMapper.MapTaskDtoAsync(context, task, cancellationToken);
	}

	private async Task<Result> ValidateRelatedEntitiesAsync(CreateTaskCommand request, CancellationToken cancellationToken)
	{
		if (await context.Teams.AsNoTracking().FirstOrDefaultAsync(team => team.Id == request.TeamId, cancellationToken) is null)
		{
			return Error.NotFound(TeamErrors.NotFoundCode, "Team not found.");
		}

		if (request.ProjectId.HasValue &&
		    !await context.Projects.AnyAsync(project => project.Id == request.ProjectId && project.TeamId == request.TeamId, cancellationToken))
		{
			return Error.NotFound(ProjectErrors.NotFoundCode, "Project not found.");
		}

		if (request.EpicId.HasValue &&
		    !await context.Epics.AnyAsync(epic => epic.Id == request.EpicId && epic.TeamId == request.TeamId, cancellationToken))
		{
			return Error.NotFound(EpicErrors.NotFoundCode, "Epic not found.");
		}

		if (request.SprintId.HasValue &&
		    !await context.Sprints.AnyAsync(sprint => sprint.Id == request.SprintId && sprint.TeamId == request.TeamId, cancellationToken))
		{
			return Error.NotFound(SprintErrors.NotFoundCode, "Sprint not found.");
		}

		if (request.AssignedTeamId.HasValue &&
		    !await context.Teams.AnyAsync(team => team.Id == request.AssignedTeamId, cancellationToken))
		{
			return Error.NotFound(TeamErrors.NotFoundCode, "Assigned team not found.");
		}

		return Result.Success();
	}

	private async Task<Result<(Guid StateId, WorkflowStateType StateType)>> ResolveWorkflowStateIdAsync(CreateTaskCommand request, CancellationToken cancellationToken)
	{
		if (request.WorkflowStateId.HasValue)
		{
			WorkflowState? workflowState = await (
				from workflow in context.Workflows.AsNoTracking()
				join state in context.WorkflowStates.AsNoTracking() on workflow.Id equals state.WorkflowId
				where workflow.TeamId == request.TeamId && state.Id == request.WorkflowStateId
				select state).FirstOrDefaultAsync(cancellationToken);

			return workflowState is null
				? Error.NotFound(WorkflowErrors.StateNotFoundCode, "Workflow state not found.")
				: Result.Success((workflowState.Id, workflowState.Type));
		}

		WorkflowState? defaultState = await (
			from workflow in context.Workflows.AsNoTracking()
			join state in context.WorkflowStates.AsNoTracking() on workflow.Id equals state.WorkflowId
			where workflow.TeamId == request.TeamId && workflow.IsDefault && state.IsDefault
			select state).FirstOrDefaultAsync(cancellationToken);

		return defaultState is null
			? Error.NotFound(WorkflowErrors.StateNotFoundCode, "Default workflow state not found.")
			: Result.Success((defaultState.Id, defaultState.Type));
	}

	private TaskItem CreateTaskEntity(
		CreateTaskCommand request,
		Guid workflowStateId,
		TeamIssueCounter issueCounter,
		Guid creatorId)
	{
		issueCounter.LastNumber++;
		List<Guid> assigneeIds = request.AssigneeIds?.ToList() ?? [];
		Guid primaryAssigneeId = request.AssigneeId ?? assigneeIds.FirstOrDefault();

		return new TaskItem
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			TeamId = request.TeamId,
			ProjectId = request.ProjectId,
			WorkflowStateId = workflowStateId,
			Number = issueCounter.LastNumber,
			Title = request.Title.Trim(),
			Description = request.Description?.Trim(),
			Priority = request.Priority,
			AssigneeId = primaryAssigneeId == Guid.Empty ? null : primaryAssigneeId,
			CreatorId = creatorId,
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
	}

	private async Task SyncAssigneesAsync(CreateTaskCommand request, Guid taskId, CancellationToken cancellationToken)
	{
		List<Guid> assigneeIds = request.AssigneeIds?.ToList() ?? [];
		Guid primaryAssigneeId = request.AssigneeId ?? assigneeIds.FirstOrDefault();

		if (assigneeIds.Count > 0)
		{
			await TaskDtoMapper.SyncAssigneesAsync(context, tenantContext.TenantId, taskId, assigneeIds, cancellationToken);
			return;
		}

		if (primaryAssigneeId != Guid.Empty)
		{
			await TaskDtoMapper.SyncAssigneesAsync(context, tenantContext.TenantId, taskId, [primaryAssigneeId], cancellationToken);
		}
	}
}
