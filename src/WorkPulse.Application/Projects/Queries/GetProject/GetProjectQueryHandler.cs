using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Queries.GetProject;

public sealed class GetProjectQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<GetProjectQuery, Result<ProjectDto>>
{
	public async Task<Result<ProjectDto>> Handle(GetProjectQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		ProjectDto? project = await (from p in context.Projects.AsNoTracking()
			join t in context.Teams.AsNoTracking() on p.TeamId equals t.Id
			where p.Id == request.ProjectId
			select new ProjectDto(p.Id, p.TeamId, t.Key, p.Name, p.Key, p.Description, p.Status.ToString(), p.LeadId, p.StartDate, p.TargetDate, p.IsArchived, p.CreatedAtUtc)).FirstOrDefaultAsync(ct);
		if (project is null)
		{
			return Error.NotFound("Project.NotFound", "Project not found.");
		}
		return project;
	}
}
