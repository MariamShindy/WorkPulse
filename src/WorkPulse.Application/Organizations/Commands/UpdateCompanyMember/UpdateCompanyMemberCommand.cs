using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Organizations.Commands.UpdateCompanyMember;

public sealed record UpdateCompanyMemberCommand(Guid MemberId, CompanyMemberRole Role, bool IsActive) : IRequest<Result>, IBaseRequest, ICommand;
