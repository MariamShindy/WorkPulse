using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Pagination;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.WorkManagement.Dtos;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.WorkManagement.Queries.ListSavedViews;

public sealed class ListSavedViewsQueryHandler(IApplicationDbContext context, ITenantContext tenantContext, ICurrentUserService currentUser) : IRequestHandler<ListSavedViewsQuery, Result<PagedList<SavedViewDto>>>
{
	public async Task<Result<PagedList<SavedViewDto>>> Handle(ListSavedViewsQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		Guid? userId = currentUser.UserId;
		IQueryable<SavedView> query = context.SavedViews.AsNoTracking().AsQueryable();
		query = ((!userId.HasValue) ? query.Where((SavedView v) => v.IsShared) : query.Where((SavedView v) => v.UserId == ((Guid?)userId).Value || v.IsShared));
		if (request.EntityType.HasValue)
		{
			query = query.Where((SavedView v) => (int)v.EntityType == (int)request.EntityType.Value);
		}
		query = query.OrderBy((SavedView v) => v.Name);
		return new PagedList<SavedViewDto>(totalCount: await query.CountAsync(ct), items: await (from v in query.Skip(request.Pagination.Skip).Take(request.Pagination.PageSize)
			select new SavedViewDto(v.Id, v.UserId, v.Name, v.EntityType.ToString(), v.FiltersJson, v.SortJson, v.IsShared, v.CreatedAtUtc)).ToListAsync(ct), page: request.Pagination.Page, pageSize: request.Pagination.PageSize);
	}
}
