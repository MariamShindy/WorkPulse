
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
		SavedView? view = await context.SavedViews.FirstOrDefaultAsync((SavedView v) => v.Id == request.SavedViewId, ct);
		if (view is null)
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
