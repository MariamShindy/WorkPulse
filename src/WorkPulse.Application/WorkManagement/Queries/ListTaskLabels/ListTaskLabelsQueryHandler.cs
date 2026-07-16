using System.Collections.Generic;
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

namespace WorkPulse.Application.WorkManagement.Queries.ListTaskLabels;

public sealed class ListTaskLabelsQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ListTaskLabelsQuery, Result<IReadOnlyList<LabelDto>>>
{
	public async Task<Result<IReadOnlyList<LabelDto>>> Handle(ListTaskLabelsQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		return await (from tl in context.TaskLabels.AsNoTracking()
			join l in context.Labels.AsNoTracking() on tl.LabelId equals l.Id
			where tl.TaskId == request.TaskId
			orderby l.Name
			select new LabelDto(l.Id, l.Name, l.Color, l.CreatedAtUtc)).ToListAsync(ct);
	}
}
