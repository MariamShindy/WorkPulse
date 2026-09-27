using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Queries.GetSprint;

public sealed class GetSprintQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<GetSprintQuery, Result<SprintDto>>
{
	public async Task<Result<SprintDto>> Handle(GetSprintQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		SprintDto? sprint = await (from s in context.Sprints.AsNoTracking()
			where s.Id == request.SprintId
			select new SprintDto(s.Id, s.TeamId, s.Name, s.Goal, s.StartDate, s.EndDate, s.Status.ToString(), s.CreatedAtUtc)).FirstOrDefaultAsync(ct);
		if (sprint is null)
		{
			return Error.NotFound("Sprint.NotFound", "Sprint not found.");
		}
		return sprint;
	}
}
