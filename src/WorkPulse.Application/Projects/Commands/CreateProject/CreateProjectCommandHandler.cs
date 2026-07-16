using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Projects.Commands.CreateProject;

public sealed class CreateProjectCommandHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<CreateProjectCommand, Result<ProjectDto>>
{
	public async Task<Result<ProjectDto>> Handle(CreateProjectCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		Team team = await context.Teams.AsNoTracking().FirstOrDefaultAsync((Team t) => t.Id == request.TeamId, ct);
		if (team == null)
		{
			return Error.NotFound("Team.NotFound", "Team not found.");
		}
		string key = (string.IsNullOrWhiteSpace(request.Key) ? SlugHelper.ToKey(request.Name) : request.Key.Trim().ToUpperInvariant());
		if (await context.Projects.AnyAsync((Project p) => p.Key == key, ct))
		{
			return Error.Conflict("Project.KeyTaken", "Project key '" + key + "' is already in use.");
		}
		Project project = new Project
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			TeamId = request.TeamId,
			Name = request.Name.Trim(),
			Key = key,
			Description = request.Description?.Trim(),
			Status = request.Status,
			LeadId = request.LeadId,
			StartDate = request.StartDate,
			TargetDate = request.TargetDate
		};
		context.Projects.Add(project);
		return new ProjectDto(project.Id, project.TeamId, team.Key, project.Name, project.Key, project.Description, project.Status.ToString(), project.LeadId, project.StartDate, project.TargetDate, project.IsArchived, project.CreatedAtUtc);
	}
}
