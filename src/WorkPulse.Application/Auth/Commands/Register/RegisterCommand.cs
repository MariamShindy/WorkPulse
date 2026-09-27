using WorkPulse.Application.Auth.Dtos;
using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Auth.Commands.Register;

public sealed record RegisterCommand(string Email, string Password, string FirstName, string LastName) : IRequest<Result<AuthResponseDto>>, IBaseRequest, ICommand;
