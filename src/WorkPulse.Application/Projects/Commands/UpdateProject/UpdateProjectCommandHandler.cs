using WorkPulse.Application.Projects.Dtos;

namespace WorkPulse.Application.Projects.Commands.UpdateProject;

public sealed class UpdateProjectCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<UpdateProjectCommand, Result<ProjectDto>>
{
	public async Task<Result<ProjectDto>> Handle(UpdateProjectCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		Project? project = await context.Projects.FirstOrDefaultAsync((Project p) => p.Id == request.ProjectId, ct);
		if (project is null)
		{
			return Error.NotFound("Project.NotFound", "Project not found.");
		}
		Team team = await context.Teams.AsNoTracking().FirstAsync((Team t) => t.Id == project.TeamId, ct);
		project.Name = request.Name.Trim();
		project.Description = request.Description?.Trim();
		project.Status = request.Status;
		project.LeadId = request.LeadId;
		project.StartDate = request.StartDate;
		project.TargetDate = request.TargetDate;
		return new ProjectDto(project.Id, project.TeamId, team.Key, project.Name, project.Key, project.Description, project.Status.ToString(), project.LeadId, project.StartDate, project.TargetDate, project.IsArchived, project.CreatedAtUtc);
	}
}
