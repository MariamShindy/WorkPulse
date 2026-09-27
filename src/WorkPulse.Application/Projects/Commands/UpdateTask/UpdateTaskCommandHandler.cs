using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Application.Projects.Services;

namespace WorkPulse.Application.Projects.Commands.UpdateTask;

public sealed class UpdateTaskCommandHandler(
	IApplicationDbContext context,
	ITenantContext tenantContext) : IRequestHandler<UpdateTaskCommand, Result<TaskItemDto>>
{
	public async Task<Result<TaskItemDto>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}

		TaskItem? task = await context.TaskItems.FirstOrDefaultAsync(item => item.Id == request.TaskId, cancellationToken);
		if (task is null)
		{
			return Error.NotFound(TaskErrors.NotFoundCode, "Task not found.");
		}

		if (request.RowVersion is not null && !task.RowVersion.SequenceEqual(request.RowVersion))
		{
			return Error.Conflict(TaskErrors.ConcurrencyCode, "The task was modified by another user. Please refresh and try again.");
		}

		Result validation = await ValidateRelatedEntitiesAsync(request, task.TeamId, cancellationToken);
		if (validation.IsFailure)
		{
			return validation.Error;
		}

		ApplyUpdates(task, request);
		await SyncAssigneesIfRequestedAsync(task, request, cancellationToken);
		return await TaskDtoMapper.MapTaskDtoAsync(context, task, cancellationToken);
	}

	private async Task<Result> ValidateRelatedEntitiesAsync(
		UpdateTaskCommand request,
		Guid teamId,
		CancellationToken cancellationToken)
	{
		if (request.ProjectId.HasValue &&
		    !await context.Projects.AnyAsync(project => project.Id == request.ProjectId && project.TeamId == teamId, cancellationToken))
		{
			return Error.NotFound(ProjectErrors.NotFoundCode, "Project not found.");
		}

		if (request.EpicId.HasValue &&
		    !await context.Epics.AnyAsync(epic => epic.Id == request.EpicId && epic.TeamId == teamId, cancellationToken))
		{
			return Error.NotFound(EpicErrors.NotFoundCode, "Epic not found.");
		}

		if (request.SprintId.HasValue &&
		    !await context.Sprints.AnyAsync(sprint => sprint.Id == request.SprintId && sprint.TeamId == teamId, cancellationToken))
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

	private static void ApplyUpdates(TaskItem task, UpdateTaskCommand request)
	{
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
	}

	private async Task SyncAssigneesIfRequestedAsync(
		TaskItem task,
		UpdateTaskCommand request,
		CancellationToken cancellationToken)
	{
		if (request.AssigneeIds is null)
		{
			return;
		}

		await TaskDtoMapper.SyncAssigneesAsync(context, tenantContext.TenantId, task.Id, request.AssigneeIds, cancellationToken);
		task.AssigneeId = request.AssigneeIds.FirstOrDefault();
	}
}
