using WorkPulse.Application.Behaviors;
using WorkPulse.Application.Organizations.Dtos;

namespace WorkPulse.Application.Organizations.Commands.AddCompanyMember;

public sealed record AddCompanyMemberCommand(Guid UserId, CompanyMemberRole Role) : IRequest<Result<CompanyMemberDto>>, IBaseRequest, ICommand;
