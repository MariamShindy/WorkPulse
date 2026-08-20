using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Queries.ListTaskAssignees;

public sealed class ListTaskAssigneesQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ListTaskAssigneesQuery, Result<IReadOnlyList<TaskAssigneeDto>>>
{
	public async Task<Result<IReadOnlyList<TaskAssigneeDto>>> Handle(ListTaskAssigneesQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		return await (from a in context.TaskAssignees.AsNoTracking()
			where a.TaskId == request.TaskId
			orderby a.CreatedAtUtc
			select new TaskAssigneeDto(a.Id, a.TaskId, a.UserId, a.CreatedAtUtc)).ToListAsync(ct);
	}
}
