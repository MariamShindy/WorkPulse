using System.Collections.Generic;
using System.Linq;
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

namespace WorkPulse.Application.Auth.Queries.ListInvitations;

public sealed class ListInvitationsQueryHandler(IApplicationDbContext context, ITenantContext tenantContext) : IRequestHandler<ListInvitationsQuery, Result<IReadOnlyList<InvitationDto>>>
{
	public async Task<Result<IReadOnlyList<InvitationDto>>> Handle(ListInvitationsQuery request, CancellationToken ct)
	{
		Result tenantCheck = tenantContext.EnsureResolved();
		if (tenantCheck.IsFailure)
		{
			return tenantCheck.Error;
		}
		return await (from i in context.UserInvitations.AsNoTracking()
			where (int)i.Status != 2
			orderby i.CreatedAtUtc descending
			select new InvitationDto(i.Id, i.Email, i.Role.ToString(), i.Status.ToString(), i.ExpiresAtUtc, i.AcceptedAtUtc, i.CreatedAtUtc)).ToListAsync(ct);
	}
}
