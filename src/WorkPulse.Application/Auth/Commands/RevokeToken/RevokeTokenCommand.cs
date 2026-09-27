using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Auth.Commands.RevokeToken;

public sealed record RevokeTokenCommand(string RefreshToken) : IRequest<Result>, IBaseRequest, ICommand;
