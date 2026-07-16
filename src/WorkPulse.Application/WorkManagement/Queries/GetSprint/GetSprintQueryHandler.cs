using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.WorkManagement.Dtos;
using WorkPulse.Domain.Entities;

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
		SprintDto sprint = await (from s in context.Sprints.AsNoTracking()
			where s.Id == request.SprintId
			select new SprintDto(s.Id, s.TeamId, s.Name, s.Goal, s.StartDate, s.EndDate, s.Status.ToString(), s.CreatedAtUtc)).FirstOrDefaultAsync(ct);
		if ((object)sprint == null)
		{
			return Error.NotFound("Sprint.NotFound", "Sprint not found.");
		}
		return sprint;
	}
}
