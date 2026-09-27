using WorkPulse.Application.Auth.Dtos;
using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Auth.Commands.AcceptInvite;

public sealed record AcceptInviteCommand(string Token, string? Password, string? FirstName, string? LastName) : IRequest<Result<AuthResponseDto>>, IBaseRequest, ICommand;
