using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Organizations.Commands.RemoveTeamMember;

public sealed record RemoveTeamMemberCommand(Guid TeamId, Guid MemberId) : IRequest<Result>, IBaseRequest, ICommand;
