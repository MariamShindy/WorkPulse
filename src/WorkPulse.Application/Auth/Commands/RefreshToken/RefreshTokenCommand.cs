using WorkPulse.Application.Auth.Dtos;
using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponseDto>>, IBaseRequest, ICommand;
