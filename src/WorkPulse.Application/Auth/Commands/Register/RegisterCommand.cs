using MediatR;
using WorkPulse.Application.Auth.Dtos;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Auth.Commands.Register;

public sealed record RegisterCommand(string Email, string Password, string FirstName, string LastName) : IRequest<Result<AuthResponseDto>>, IBaseRequest, ICommand;
