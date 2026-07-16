using MediatR;
using WorkPulse.Application.Auth.Dtos;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Auth.Queries.GetProfile;

public sealed record GetProfileQuery : IRequest<Result<UserProfileDto>>, IBaseRequest, IQuery;
