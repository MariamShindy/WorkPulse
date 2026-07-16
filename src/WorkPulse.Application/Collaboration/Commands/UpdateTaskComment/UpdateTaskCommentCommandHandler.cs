using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Collaboration.Dtos;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Collaboration.Commands.UpdateTaskComment;

public sealed class UpdateTaskCommentCommandHandler(IApplicationDbContext context, ITenantContext tenantContext, ICurrentUserService currentUser) : IRequestHandler<UpdateTaskCommentCommand, Result<TaskCommentDto>>
{
	public async Task<Result<TaskCommentDto>> Handle(UpdateTaskCommentCommand request, CancellationToken ct)
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
		TaskComment comment = await context.TaskComments.FirstOrDefaultAsync((TaskComment c) => c.Id == request.CommentId, ct);
		if (comment == null)
		{
			return Error.NotFound("Collaboration.CommentNotFound", "Comment not found.");
		}
		if (comment.AuthorId != currentUser.UserId.Value)
		{
			return Error.Forbidden("Comment.Forbidden", "You can only edit your own comments.");
		}
		comment.Body = request.Body.Trim();
		comment.IsEdited = true;
		return new TaskCommentDto(comment.Id, comment.TaskId, comment.AuthorId, comment.Body, comment.MentionedUserIds, comment.IsEdited, comment.CreatedAtUtc, comment.UpdatedAtUtc);
	}
}
