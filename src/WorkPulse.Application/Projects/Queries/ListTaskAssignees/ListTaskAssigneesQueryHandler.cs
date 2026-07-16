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
using WorkPulse.Application.Projects.Dtos;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.Projects.Queries.ListTaskAssignees;

public sealed class ListTaskAssigneesQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ListTaskAssigneesQuery, Result<IReadOnlyList<TaskAssigneeDto>>>
{
	public async Task<Result<IReadOnlyList<TaskAssigneeDto>>> Handle(ListTaskAssigneesQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		return await (from a in context.TaskAssignees.AsNoTracking()
			where a.TaskId == request.TaskId
			orderby a.CreatedAtUtc
			select new TaskAssigneeDto(a.Id, a.TaskId, a.UserId, a.CreatedAtUtc)).ToListAsync(ct);
	}
}
