
namespace WorkPulse.Application.Projects.Commands.ArchiveProject;

public sealed class ArchiveProjectCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ArchiveProjectCommand, Result>
{
	public async Task<Result> Handle(ArchiveProjectCommand request, CancellationToken ct)
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
		project.IsArchived = true;
		return Result.Success();
	}
}
