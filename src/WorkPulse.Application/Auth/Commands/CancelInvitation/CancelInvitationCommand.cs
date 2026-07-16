using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;

namespace WorkPulse.Application.Auth.Commands.CancelInvitation;

public sealed record CancelInvitationCommand(Guid InvitationId) : IRequest<Result>, IBaseRequest, ICommand;
