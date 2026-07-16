using System.Collections.Generic;
using MediatR;
using WorkPulse.Application.Auth.Dtos;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Auth.Queries.ListInvitations;

public sealed record ListInvitationsQuery : IRequest<Result<IReadOnlyList<InvitationDto>>>, IBaseRequest, IQuery;
