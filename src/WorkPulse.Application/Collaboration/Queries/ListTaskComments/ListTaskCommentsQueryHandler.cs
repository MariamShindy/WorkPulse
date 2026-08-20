using WorkPulse.Application.Collaboration.Dtos;
using WorkPulse.Application.Common.Pagination;

namespace WorkPulse.Application.Collaboration.Queries.ListTaskComments;

public sealed class ListTaskCommentsQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ListTaskCommentsQuery, Result<PagedList<TaskCommentDto>>>
{
	public async Task<Result<PagedList<TaskCommentDto>>> Handle(ListTaskCommentsQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		IOrderedQueryable<TaskComment> query = from c in context.TaskComments.AsNoTracking()
			where c.TaskId == request.TaskId
			orderby c.CreatedAtUtc descending
			select c;
		return new PagedList<TaskCommentDto>(totalCount: await query.CountAsync(ct), items: await (from c in query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize)
			select new TaskCommentDto(c.Id, c.TaskId, c.AuthorId, c.Body, c.MentionedUserIds, c.IsEdited, c.CreatedAtUtc, c.UpdatedAtUtc)).ToListAsync(ct), page: request.Pagination.Page, pageSize: request.Pagination.PageSize);
	}
}
