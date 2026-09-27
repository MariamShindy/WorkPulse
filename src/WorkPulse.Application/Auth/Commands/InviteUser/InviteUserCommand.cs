using WorkPulse.Application.Auth.Dtos;
using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Auth.Commands.InviteUser;

public sealed record InviteUserCommand(string Email, CompanyMemberRole Role) : IRequest<Result<InvitationDto>>, IBaseRequest, ICommand;
