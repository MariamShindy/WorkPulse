using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WorkPulse.Application.Abstractions;
using WorkPulse.Application.Abstractions.Persistence;
using WorkPulse.Application.Auth.Dtos;
using WorkPulse.Application.Common.Extensions;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Entities;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Auth.Commands.InviteUser;

public sealed class InviteUserCommandHandler(IApplicationDbContext context, ITenantContext tenantContext, ICurrentUserService currentUser, IDateTime dateTime, IEmailService emailService) : IRequestHandler<InviteUserCommand, Result<InvitationDto>>
{
	public async Task<Result<InvitationDto>> Handle(InviteUserCommand request, CancellationToken ct)
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
		string normalizedEmail = request.Email.Trim().ToLowerInvariant();
		if (await context.UserInvitations.AnyAsync((UserInvitation i) => i.Email == normalizedEmail && (int)i.Status == 0 && i.ExpiresAtUtc > dateTime.UtcNow, ct))
		{
			return Error.Conflict("Invitation.PendingExists", "A pending invitation already exists for this email.");
		}
		string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));
		DateTime now = dateTime.UtcNow;
		UserInvitation invitation = new UserInvitation
		{
			Id = Guid.NewGuid(),
			TenantId = tenantContext.TenantId,
			Email = normalizedEmail,
			Role = request.Role,
			Token = token,
			ExpiresAtUtc = now.AddDays(7.0),
			InvitedById = currentUser.UserId.Value,
			Status = InvitationStatus.Pending,
			CreatedAtUtc = now
		};
		context.UserInvitations.Add(invitation);
		await emailService.SendAsync(normalizedEmail, "You're invited to join WorkPulse", "<p>You have been invited to join a company on WorkPulse.</p><p>Use this token to accept: <strong>" + token + "</strong></p>", ct);
		return ToDto(invitation);
	}

	internal static InvitationDto ToDto(UserInvitation invitation)
	{
		return new InvitationDto(invitation.Id, invitation.Email, invitation.Role.ToString(), invitation.Status.ToString(), invitation.ExpiresAtUtc, invitation.AcceptedAtUtc, invitation.CreatedAtUtc);
	}
}
