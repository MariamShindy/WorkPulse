using WorkPulse.Application.WorkManagement.Dtos;

namespace WorkPulse.Application.WorkManagement.Queries.GetSavedView;

public sealed class GetSavedViewQueryHandler(IApplicationDbContext context, ITenantContext tenantContext, ICurrentUserService currentUser) : IRequestHandler<GetSavedViewQuery, Result<SavedViewDto>>
{
	public async Task<Result<SavedViewDto>> Handle(GetSavedViewQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		SavedViewDto? view = await (from v in context.SavedViews.AsNoTracking()
			where v.Id == request.SavedViewId
			select new SavedViewDto(v.Id, v.UserId, v.Name, v.EntityType.ToString(), v.FiltersJson, v.SortJson, v.IsShared, v.CreatedAtUtc)).FirstOrDefaultAsync(ct);
		if (view is null)
		{
			return Error.NotFound("SavedView.NotFound", "Saved view not found.");
		}
		if (currentUser.UserId.HasValue && view.UserId != currentUser.UserId.Value && !view.IsShared)
		{
			return Error.Forbidden("SavedView.Forbidden", "You do not have access to this saved view.");
		}
		return view;
	}
}
