using WorkPulse.Application.Auth.Dtos;
using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Auth.Queries.ListInvitations;

public sealed record ListInvitationsQuery : IRequest<Result<IReadOnlyList<InvitationDto>>>, IBaseRequest, IQuery;
