using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.WorkManagement.Commands.DeleteSavedView;

public sealed class DeleteSavedViewCommandHandler(IApplicationDbContext context, ITenantContext tenantContext, ICurrentUserService currentUser) : IRequestHandler<DeleteSavedViewCommand, Result>
{
	public async Task<Result> Handle(DeleteSavedViewCommand request, CancellationToken ct)
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
			return Error.Forbidden("SavedView.Forbidden", "You can only delete your own saved views.");
		}
		context.SavedViews.Remove(view);
		return Result.Success();
	}
}
