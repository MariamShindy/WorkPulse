using System;
using MediatR;
using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Common.Result;
using WorkPulse.Application.Organizations.Dtos;
using WorkPulse.Domain.Enums;

namespace WorkPulse.Application.Organizations.Commands.AddCompanyMember;

public sealed record AddCompanyMemberCommand(Guid UserId, CompanyMemberRole Role) : IRequest<Result<CompanyMemberDto>>, IBaseRequest, ICommand;
