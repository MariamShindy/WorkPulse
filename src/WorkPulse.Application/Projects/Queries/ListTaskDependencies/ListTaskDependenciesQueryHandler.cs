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

namespace WorkPulse.Application.Projects.Queries.ListTaskDependencies;

public sealed class ListTaskDependenciesQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ListTaskDependenciesQuery, Result<IReadOnlyList<TaskDependencyDto>>>
{
	public async Task<Result<IReadOnlyList<TaskDependencyDto>>> Handle(ListTaskDependenciesQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		return await (from d in context.TaskDependencies.AsNoTracking()
			where d.TaskId == request.TaskId
			orderby d.CreatedAtUtc
			select new TaskDependencyDto(d.Id, d.TaskId, d.DependsOnTaskId, d.Type.ToString(), d.CreatedAtUtc)).ToListAsync(ct);
	}
}
