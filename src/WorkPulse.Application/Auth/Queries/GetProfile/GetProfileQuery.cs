using WorkPulse.Application.Auth.Dtos;
using WorkPulse.Application.Behaviors;

namespace WorkPulse.Application.Auth.Queries.GetProfile;

public sealed record GetProfileQuery : IRequest<Result<UserProfileDto>>, IBaseRequest, IQuery;
