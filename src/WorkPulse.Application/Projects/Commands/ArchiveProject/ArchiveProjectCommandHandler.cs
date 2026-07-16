using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;

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
		Project project = await context.Projects.FirstOrDefaultAsync((Project p) => p.Id == request.ProjectId, ct);
		if (project == null)
		{
			return Error.NotFound("Project.NotFound", "Project not found.");
		}
		project.IsArchived = true;
		return Result.Success();
	}
}
