using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Queries.GetEpic;

public sealed class GetEpicQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<GetEpicQuery, Result<EpicDto>>
{
	public async Task<Result<EpicDto>> Handle(GetEpicQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		EpicDto? epic = await (from e in context.Epics.AsNoTracking()
			where e.Id == request.EpicId
			select new EpicDto(e.Id, e.TeamId, e.Title, e.Description, e.Status.ToString(), e.CreatedAtUtc)).FirstOrDefaultAsync(ct);
		if (epic is null)
		{
			return Error.NotFound("Epic.NotFound", "Epic not found.");
		}
		return epic;
	}
}
