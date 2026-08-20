using Microsoft.AspNetCore.Authorization;
using WorkPulse.API.Extensions;
using WorkPulse.API.Filters;
using WorkPulse.Application.Collaboration.Commands.AddTaskComment;
using WorkPulse.Application.Collaboration.Commands.MarkNotificationRead;
using WorkPulse.Application.Collaboration.Commands.UnwatchTask;
using WorkPulse.Application.Collaboration.Commands.UpdateTaskComment;
using WorkPulse.Application.Collaboration.Commands.WatchTask;
using WorkPulse.Application.Collaboration.Dtos;
using WorkPulse.Application.Collaboration.Queries.ListNotifications;
using WorkPulse.Application.Collaboration.Queries.ListTaskActivity;
using WorkPulse.Application.Collaboration.Queries.ListTaskComments;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.API.Contracts.Collaboration;

namespace WorkPulse.API.Controllers;

[ApiController]
[Authorize(Policy = "RequireAuthenticated")]
[TenantRequired]
public sealed class CollaborationController(ISender sender) : ControllerBase
{
	[HttpGet("api/tasks/{taskId:guid}/comments")]
	public async Task<ActionResult<PagedList<TaskCommentDto>>> ListComments(Guid taskId, [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<PagedList<TaskCommentDto>>>)new ListTaskCommentsQuery(taskId, new PaginationParams
		{
			Page = page,
			PageSize = pageSize
		}), ct)).ToActionResult();
	}

	[HttpPost("api/tasks/{taskId:guid}/comments")]
	public async Task<ActionResult<TaskCommentDto>> AddComment(Guid taskId, [FromBody] AddCommentRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<TaskCommentDto>>)new AddTaskCommentCommand(taskId, request.Body, request.MentionedUserIds), ct)).ToActionResult();
	}

	[HttpPut("api/comments/{commentId:guid}")]
	public async Task<ActionResult<TaskCommentDto>> UpdateComment(Guid commentId, [FromBody] UpdateCommentRequest request, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result<TaskCommentDto>>)new UpdateTaskCommentCommand(commentId, request.Body), ct)).ToActionResult();
	}

	[HttpGet("api/tasks/{taskId:guid}/activity")]
	public async Task<ActionResult<PagedList<TaskActivityDto>>> ListActivity(Guid taskId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<PagedList<TaskActivityDto>>>)new ListTaskActivityQuery(taskId, new PaginationParams
		{
			Page = page,
			PageSize = pageSize
		}), ct)).ToActionResult();
	}

	[HttpPost("api/tasks/{taskId:guid}/watch")]
	public async Task<IActionResult> Watch(Guid taskId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new WatchTaskCommand(taskId), ct)).ToActionResult();
	}

	[HttpDelete("api/tasks/{taskId:guid}/watch")]
	public async Task<IActionResult> Unwatch(Guid taskId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new UnwatchTaskCommand(taskId), ct)).ToActionResult();
	}

	[HttpGet("api/notifications")]
	public async Task<ActionResult<PagedList<NotificationDto>>> ListNotifications([FromQuery] bool unreadOnly = false, [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default(CancellationToken))
	{
		return (await sender.Send((IRequest<Result<PagedList<NotificationDto>>>)new ListNotificationsQuery(new PaginationParams
		{
			Page = page,
			PageSize = pageSize
		}, unreadOnly), ct)).ToActionResult();
	}

	[HttpPost("api/notifications/{notificationId:guid}/read")]
	public async Task<IActionResult> MarkRead(Guid notificationId, CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new MarkNotificationReadCommand(notificationId), ct)).ToActionResult();
	}

	[HttpPost("api/notifications/read-all")]
	public async Task<IActionResult> MarkAllRead(CancellationToken ct)
	{
		return (await sender.Send((IRequest<Result>)new MarkAllNotificationsReadCommand(), ct)).ToActionResult();
	}
}
