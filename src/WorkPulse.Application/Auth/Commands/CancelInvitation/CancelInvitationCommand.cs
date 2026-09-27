using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Auth.Commands.CancelInvitation;

public sealed record CancelInvitationCommand(Guid InvitationId) : IRequest<Result>, IBaseRequest, ICommand;
