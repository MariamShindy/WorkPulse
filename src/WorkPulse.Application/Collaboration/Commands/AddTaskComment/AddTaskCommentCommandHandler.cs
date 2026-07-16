using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Collaboration.Dtos;
using WorkPulse.Application.Collaboration.Services;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Collaboration.Commands.AddTaskComment;

public sealed class AddTaskCommentCommandHandler(IApplicationDbContext context, ITenantContext tenantContext, ICurrentUserService currentUser, ITaskCollaborationService collaboration) : IRequestHandler<AddTaskCommentCommand, Result<TaskCommentDto>>
{
	public async Task<Result<TaskCommentDto>> Handle(AddTaskCommentCommand request, CancellationToken ct)
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
		if (await context.TaskItems.AsNoTracking().FirstOrDefaultAsync((TaskItem t) => t.Id == request.TaskId, ct) == null)
		{
			return Error.NotFound("Task.NotFound", "Task not found.");
		}
		List<Guid> mentions = request.MentionedUserIds?.Distinct().ToList() ?? new List<Guid>();
		TaskComment comment = new TaskComment
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			TaskId = request.TaskId,
			AuthorId = currentUser.UserId.Value,
			Body = request.Body.Trim(),
			MentionedUserIds = mentions
		};
		context.TaskComments.Add(comment);
		await collaboration.RecordActivityAsync(tenantContext.TenantId, request.TaskId, currentUser.UserId.Value, ActivityType.Commented, "Added a comment", new
		{
			commentId = comment.Id
		}, ct);
		if (mentions.Count > 0)
		{
			await collaboration.NotifyUsersAsync(tenantContext.TenantId, mentions, NotificationType.TaskMention, "You were mentioned", request.Body.Trim().Substring(0, Math.Min(120, request.Body.Trim().Length)), currentUser.UserId.Value, "Task", request.TaskId, ct);
		}
		await collaboration.NotifyWatchersAsync(tenantContext.TenantId, request.TaskId, NotificationType.TaskComment, "New comment on watched task", request.Body.Trim().Substring(0, Math.Min(120, request.Body.Trim().Length)), currentUser.UserId.Value, mentions.Append(currentUser.UserId.Value), ct);
		return new TaskCommentDto(comment.Id, comment.TaskId, comment.AuthorId, comment.Body, comment.MentionedUserIds, comment.IsEdited, comment.CreatedAtUtc, comment.UpdatedAtUtc);
	}
}
