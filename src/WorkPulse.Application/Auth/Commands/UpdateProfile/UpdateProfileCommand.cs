using WorkPulse.Application.Auth.Dtos;
using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Auth.Commands.UpdateProfile;

public sealed record UpdateProfileCommand(string FirstName, string LastName, string? AvatarUrl) : IRequest<Result<UserProfileDto>>, IBaseRequest, ICommand;
