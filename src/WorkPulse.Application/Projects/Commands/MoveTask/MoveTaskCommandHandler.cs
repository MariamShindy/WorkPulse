using WorkPulse.Application.IntegrationEvents;
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Application.Projects.Services;

namespace WorkPulse.Application.Projects.Commands.MoveTask;

public sealed class MoveTaskCommandHandler(IApplicationDbContext context, ITenantContext tenantContext, IAutomationEngine automationEngine, IOutboxWriter outboxWriter, ITaskRealtimeNotifier realtimeNotifier) : IRequestHandler<MoveTaskCommand, Result<TaskItemDto>>
{
	public async Task<Result<TaskItemDto>> Handle(MoveTaskCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		TaskItem? task = await context.TaskItems.FirstOrDefaultAsync((TaskItem t) => t.Id == request.TaskId, ct);
		if (task is null)
		{
			return Error.NotFound("Task.NotFound", "Task not found.");
		}
		WorkflowStateType? newStateType = await (from w in context.Workflows.AsNoTracking()
			join s in context.WorkflowStates.AsNoTracking() on w.Id equals s.WorkflowId
			where w.TeamId == task.TeamId && s.Id == request.WorkflowStateId
			select (WorkflowStateType?)s.Type).FirstOrDefaultAsync(ct);
		if (newStateType is null)
		{
			return Error.Validation("Task.InvalidState", "Workflow state does not belong to this task's team.");
		}
		Guid previousStateId = task.WorkflowStateId;
		task.WorkflowStateId = request.WorkflowStateId;
		WorkflowStateTransition.Apply(task, newStateType.Value);
		if (request.SortOrder.HasValue)
		{
			task.SortOrder = request.SortOrder.Value;
		}
		await automationEngine.EvaluateTaskStatusChangeAsync(task, previousStateId, ct);
		await outboxWriter.EnqueueAsync(new TaskStatusChangedIntegrationEvent(task.TenantId, task.Id, previousStateId, task.WorkflowStateId), ct);
		TaskItemDto dto = await TaskDtoMapper.MapTaskDtoAsync(context, task, ct);
		await realtimeNotifier.NotifyTaskMovedAsync(tenantContext.TenantId, task.TeamId, task.Id, dto, ct);
		return dto;
	}
}
