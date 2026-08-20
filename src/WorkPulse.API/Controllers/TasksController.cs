using Microsoft.AspNetCore.Authorization;
using WorkPulse.API.Extensions;
using WorkPulse.API.Filters;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Projects.Commands.AddTaskAssignee;
using WorkPulse.Application.Projects.Commands.AddTaskDependency;
using WorkPulse.Application.Projects.Commands.CreateTask;
using WorkPulse.Application.Projects.Commands.DeleteTask;
using WorkPulse.Application.Projects.Commands.MoveTask;
using WorkPulse.Application.Projects.Commands.RemoveTaskAssignee;
using WorkPulse.Application.Projects.Commands.RemoveTaskDependency;
using WorkPulse.Application.Projects.Commands.UpdateTask;
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Application.Projects.Queries.GetTask;
using WorkPulse.Application.Projects.Queries.ListTaskAssignees;
using WorkPulse.Application.Projects.Queries.ListTaskDependencies;
using WorkPulse.Application.Projects.Queries.ListTasks;
using WorkPulse.Application.WorkManagement.Commands.AssignLabelToTask;
using WorkPulse.Application.WorkManagement.Commands.RemoveLabelFromTask;
using WorkPulse.Application.WorkManagement.Dtos;
using WorkPulse.Application.WorkManagement.Queries.ListTaskLabels;
using WorkPulse.Domain.Enums;
using WorkPulse.API.Contracts.Projects;
using WorkPulse.API.Contracts.WorkManagement;

namespace WorkPulse.API.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize(Policy = "RequireAuthenticated")]
[TenantRequired]
public sealed class TasksController(ISender sender) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<PagedList<TaskItemDto>>> List([FromQuery] Guid? teamId, [FromQuery] Guid? projectId, [FromQuery] Guid? assigneeId, [FromQuery] Guid? workflowStateId, [FromQuery] TaskPriority? priority, [FromQuery] string? sortBy = null, [FromQuery] string sortDirection = "asc", [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<PagedList<TaskItemDto>>>)new ListTasksQuery(new PaginationParams
		{
			Page = page,
			PageSize = pageSize
		}, teamId, projectId, assigneeId, workflowStateId, priority), ct)).ToActionResult();
	}

	[HttpGet("{taskId:guid}")]
	public async Task<ActionResult<TaskItemDto>> Get(Guid taskId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<TaskItemDto>>)new GetTaskQuery(taskId), ct)).ToActionResult();
	}

	[HttpPost]
	public async Task<ActionResult<TaskItemDto>> Create([FromBody] CreateTaskRequest request, CancellationToken ct)
	{
		Result<TaskItemDto> result = await sender.Send((IRequest<Result<TaskItemDto>>)new CreateTaskCommand(request.TeamId, request.Title, request.Description, request.Priority, request.ProjectId, request.WorkflowStateId, request.AssigneeId, request.AssigneeIds, request.DueDate, request.ParentTaskId, request.StoryPoints, request.EstimatedHours, request.IsBlocked, request.BlockedReason, request.EpicId, request.SprintId, request.AssignedTeamId), ct);
		if (result.IsFailure)
		{
			return result.ToActionResult();
		}
		return CreatedAtAction("Get", new
		{
			taskId = result.Value.Id
		}, result.Value);
	}

	[HttpPut("{taskId:guid}")]
	public async Task<ActionResult<TaskItemDto>> Update(Guid taskId, [FromBody] UpdateTaskRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<TaskItemDto>>)new UpdateTaskCommand(taskId, request.Title, request.Description, request.Priority, request.ProjectId, request.AssigneeId, request.AssigneeIds, request.DueDate, request.StoryPoints, request.EstimatedHours, request.IsBlocked, request.BlockedReason, request.EpicId, request.SprintId, request.AssignedTeamId, request.RowVersion), ct)).ToActionResult();
	}

	[HttpPost("{taskId:guid}/move")]
	public async Task<ActionResult<TaskItemDto>> Move(Guid taskId, [FromBody] MoveTaskRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<TaskItemDto>>)new MoveTaskCommand(taskId, request.WorkflowStateId, request.SortOrder), ct)).ToActionResult();
	}

	[HttpDelete("{taskId:guid}")]
	public async Task<IActionResult> Delete(Guid taskId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new DeleteTaskCommand(taskId), ct)).ToActionResult();
	}

	[HttpGet("{taskId:guid}/assignees")]
	public async Task<ActionResult<IReadOnlyList<TaskAssigneeDto>>> ListAssignees(Guid taskId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<IReadOnlyList<TaskAssigneeDto>>>)new ListTaskAssigneesQuery(taskId), ct)).ToActionResult();
	}

	[HttpPost("{taskId:guid}/assignees")]
	public async Task<IActionResult> AddAssignee(Guid taskId, [FromBody] AddTaskAssigneeRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new AddTaskAssigneeCommand(taskId, request.UserId), ct)).ToActionResult();
	}

	[HttpDelete("{taskId:guid}/assignees/{userId:guid}")]
	public async Task<IActionResult> RemoveAssignee(Guid taskId, Guid userId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new RemoveTaskAssigneeCommand(taskId, userId), ct)).ToActionResult();
	}

	[HttpGet("{taskId:guid}/dependencies")]
	public async Task<ActionResult<IReadOnlyList<TaskDependencyDto>>> ListDependencies(Guid taskId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<IReadOnlyList<TaskDependencyDto>>>)new ListTaskDependenciesQuery(taskId), ct)).ToActionResult();
	}

	[HttpPost("{taskId:guid}/dependencies")]
	public async Task<ActionResult<TaskDependencyDto>> AddDependency(Guid taskId, [FromBody] AddTaskDependencyRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<TaskDependencyDto>>)new AddTaskDependencyCommand(taskId, request.DependsOnTaskId, request.Type), ct)).ToActionResult();
	}

	[HttpDelete("{taskId:guid}/dependencies/{dependencyId:guid}")]
	public async Task<IActionResult> RemoveDependency(Guid taskId, Guid dependencyId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new RemoveTaskDependencyCommand(dependencyId), ct)).ToActionResult();
	}

	[HttpGet("{taskId:guid}/labels")]
	public async Task<ActionResult<IReadOnlyList<LabelDto>>> ListLabels(Guid taskId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<IReadOnlyList<LabelDto>>>)new ListTaskLabelsQuery(taskId), ct)).ToActionResult();
	}

	[HttpPost("{taskId:guid}/labels")]
	public async Task<IActionResult> AssignLabel(Guid taskId, [FromBody] AssignLabelRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new AssignLabelToTaskCommand(taskId, request.LabelId), ct)).ToActionResult();
	}

	[HttpDelete("{taskId:guid}/labels/{labelId:guid}")]
	public async Task<IActionResult> RemoveLabel(Guid taskId, Guid labelId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new RemoveLabelFromTaskCommand(taskId, labelId), ct)).ToActionResult();
	}
}
