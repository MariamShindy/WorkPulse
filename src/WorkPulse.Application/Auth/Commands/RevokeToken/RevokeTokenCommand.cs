using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Auth.Commands.RevokeToken;

public sealed record RevokeTokenCommand(string RefreshToken) : IRequest<Result>, IBaseRequest, ICommand;
