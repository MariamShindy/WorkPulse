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

namespace WorkPulse.Application.WorkManagement.Commands.UpdateSavedView;

public sealed class UpdateSavedViewCommandHandler(IApplicationDbContext context, ITenantContext tenantContext, ICurrentUserService currentUser) : IRequestHandler<UpdateSavedViewCommand, Result<SavedViewDto>>
{
	public async Task<Result<SavedViewDto>> Handle(UpdateSavedViewCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
		{
			return Error.Unauthorized("Auth.Unauthorized", "Authentication is required.");
		}
		SavedView view = await context.SavedViews.FirstOrDefaultAsync((SavedView v) => v.Id == request.SavedViewId, ct);
		if (view == null)
		{
			return Error.NotFound("SavedView.NotFound", "Saved view not found.");
		}
		if (view.UserId != currentUser.UserId.Value)
		{
			return Error.Forbidden("SavedView.Forbidden", "You can only update your own saved views.");
		}
		view.Name = request.Name.Trim();
		view.EntityType = request.EntityType;
		view.FiltersJson = request.FiltersJson;
		view.SortJson = request.SortJson;
		view.IsShared = request.IsShared;
		return new SavedViewDto(view.Id, view.UserId, view.Name, view.EntityType.ToString(), view.FiltersJson, view.SortJson, view.IsShared, view.CreatedAtUtc);
	}
}
