using MediatR;
using WorkPulse.Application.Auth.Dtos;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Auth.Commands.InviteUser;

public sealed record InviteUserCommand(string Email, CompanyMemberRole Role) : IRequest<Result<InvitationDto>>, IBaseRequest, ICommand;
