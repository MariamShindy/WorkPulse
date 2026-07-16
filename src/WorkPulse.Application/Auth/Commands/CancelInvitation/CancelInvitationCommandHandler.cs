using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Auth.Commands.CancelInvitation;

public sealed class CancelInvitationCommandHandler(IApplicationDbContext context, ITenantContext tenantContext, IDateTime dateTime) : IRequestHandler<CancelInvitationCommand, Result>
{
	public async Task<Result> Handle(CancelInvitationCommand request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		UserInvitation invitation = await context.UserInvitations.FirstOrDefaultAsync((UserInvitation i) => i.Id == request.InvitationId, ct);
		if (invitation == null)
		{
			return Error.NotFound("Invitation.NotFound", "Invitation not found.");
		}
		if (invitation.Status == InvitationStatus.Accepted)
		{
			return Error.Conflict("Invitation.AlreadyAccepted", "Cannot cancel an accepted invitation.");
		}
		invitation.Status = InvitationStatus.Cancelled;
		invitation.UpdatedAtUtc = dateTime.UtcNow;
		return Result.Success();
	}
}
