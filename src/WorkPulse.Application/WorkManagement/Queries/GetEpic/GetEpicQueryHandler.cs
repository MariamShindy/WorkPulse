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
		EpicDto epic = await (from e in context.Epics.AsNoTracking()
			where e.Id == request.EpicId
			select new EpicDto(e.Id, e.TeamId, e.Title, e.Description, e.Status.ToString(), e.CreatedAtUtc)).FirstOrDefaultAsync(ct);
		if ((object)epic == null)
		{
			return Error.NotFound("Epic.NotFound", "Epic not found.");
		}
		return epic;
	}
}
