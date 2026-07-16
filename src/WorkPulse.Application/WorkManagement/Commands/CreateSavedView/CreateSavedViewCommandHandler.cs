using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.WorkManagement.Dtos;
using WorkPulse.Domain.Entities;

namespace WorkPulse.Application.WorkManagement.Commands.CreateSavedView;

public sealed class CreateSavedViewCommandHandler(IApplicationDbContext context, ITenantContext tenantContext, ICurrentUserService currentUser) : IRequestHandler<CreateSavedViewCommand, Result<SavedViewDto>>
{
	public async Task<Result<SavedViewDto>> Handle(CreateSavedViewCommand request, CancellationToken ct)
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
		SavedView view = new SavedView
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			UserId = currentUser.UserId.Value,
			Name = request.Name.Trim(),
			EntityType = request.EntityType,
			FiltersJson = request.FiltersJson,
			SortJson = request.SortJson,
			IsShared = request.IsShared
		};
		context.SavedViews.Add(view);
		return new SavedViewDto(view.Id, view.UserId, view.Name, view.EntityType.ToString(), view.FiltersJson, view.SortJson, view.IsShared, view.CreatedAtUtc);
	}
}
